using System;
using System.Collections.Generic;
using System.Linq;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Sync.Integrators
{
    public class RotaSync : Sync<RotaDto>
    {
        public RotaSync(ConfiguracaoDto configuracao, IDbSession session = null)
            : base(DbHelper.GetOnline<IRotaDb>(), DbHelper.GetOffline<IOfflineRotaDb>(session), "RotaIntegra", configuracao)
        {
            IntervaloIntegracao = configuracao.IntervaloSyncRota;
        }

        protected override void DoExecuteSync(DateTime dataUltimaIntegracao, DateTime inicioIntegracao, UsuarioDto usuario)
        {
            // Se a propriedade do config IsIndiceInicialDiaModificado for verdadeira

            // O circulo iniciará do indice com a propriedade IndiceInicialDia e dia será o dia corrente.

            // Senão

            // Obter o ultimo bloco inicial e final. Retorna um array de rotas 
            // bloco[0] - rota no bloco inicial
            // bloco[1] - rota no bloco final

            // Verificar se o bloco final está com o dia menor que hoje.

            // Se sim, continuar o processo para o proximo dia do bloco final, e circular as pastas para o proximo indice do indice do bloco final.
            // Bloco final - Dia: 01/01 - Indice: 10
            // Circulo das pastas - 11 pastas
            // Inicio cursor - pasta 10
            // Próximo cursor - pasta 11 - dia 02/01 - Inicio processo
            // Próximo cursor - pasta 01 - dia 03/01
            // Próximo cursor - pasta 02 - dia 04/01
            // ....
            // Ultimo cursor - pasta 10 - dia 12/01


            // Se náo, reprocessar o bloco, do bloco inicial, circulando as pasta.
            // Bloco inicial - Dia: 01/01 - Indice: 10
            // Bloco final - Dia: 10/01 - Indice: 10
            // Circulo das pastas - 11 pastas
            // Inicio cursor - pasta 01
            // Próximo cursor - pasta 01 - dia 01/01 - Inicio processo
            // ....
            // Ultimo cursor - pasta 11 - dia 11/01

            // Senão tiver bloco inicial e final

            // Iniciar o circulo do primeiro indice das pastas obtidas e dia corrente.

            // Sempre atualizar as rotas com o bloco inicial e final.
            // Primeira e ultima rota obtida do metodo GetRotasPastas.
            try
            {
                var rotaDb = (IOfflineRotaDb)Offline;
                var clienteDb = DbHelper.GetOffline<IOfflineClienteDb>();
                var configuracaoDb = DbHelper.GetOfflineConfiguracaoDb();
                var configuracao = configuracaoDb.GetConfiguracaoAtiva();
                var rotaOnlineDb = (IRotaDb)Online;
                var pastas = rotaOnlineDb.GetPastas(usuario);
                IEnumerable<RotaDto> dtosOnline = null;

                if (pastas.Count() <= 0)
                {
                    // Não existem pastas a serem processadas.
                    Logger.Warn(true, "Não existem pastas a serem processadas para o usuário: {0}", usuario.Username);
                    return;
                }

                if (configuracao.IsIndiceInicialDiaModificado) // Forçar o indice da pasta inicial para uma pasta e um dia configurado.
                {
                    dtosOnline = GetRotasPastas(pastas, usuario, configuracao.IndiceInicialDia, DateTime.Today);
                }
                else
                {
                    Logger.Debug("Verificar bloco");
                    var bloco = rotaDb.GetUltimoBloco(usuario);
                    if (bloco.Count() > 0) // Se existir um bloco gravado
                    {
                        Logger.Debug("Existe bloco - {0} ({1}): {2} - {3} ({4}): {5} ", bloco[0].Bloco,
                                     bloco[0].IndicePasta, bloco[0].Dia, bloco[1].Bloco, bloco[1].IndicePasta,
                                     bloco[1].Dia);
                        dtosOnline = DateTime.Today > bloco[1].Dia ?
                            GetRotasPastas(pastas, usuario, bloco[1].IndicePasta, bloco[1].Dia.AddDays(1), true) :
                            GetRotasPastas(pastas, usuario, bloco[0].IndicePasta, bloco[0].Dia);
                    }
                    else
                    {
                        Logger.Debug("Ñ Existe bloco");
                        int menorIndice = pastas.Min(p => p.Indice);
                        dtosOnline = GetRotasPastas(pastas, usuario, menorIndice, GetDateOfPasta(menorIndice, pastas.Count()));
                    }
                }

                foreach (var rotaERP in dtosOnline)
                {
                    var rotaLocal = rotaDb.GetRota(rotaERP.Dia, rotaERP.Usuario);
                    bool isRotaNova = false;
                    // TODO: Verificar se tem informações da rota para atualizar.
                    if (rotaLocal == null) // Salva uma nova rota.
                    {
                        rotaERP.DataCriacao = DateTime.Now;
                        rotaDb.Save(rotaERP);
                        rotaLocal = rotaERP;
                        isRotaNova = true;
                    }
                    else
                    {
                        // Atualizar rota
                        rotaLocal.IndicePasta = rotaERP.IndicePasta;
                        rotaLocal.Nome = rotaERP.Nome;
                        rotaLocal.DataAlteracao = DateTime.Now;
                        rotaDb.Save(rotaLocal);
                        rotaERP.Id = rotaLocal.Id;

                        // Verificar os clientes que não vieram do ERP e colocar como desabilitado.
                        foreach (var clienteForaRoteiro in rotaLocal.Clientes.Where(clienteFora => !rotaERP.Clientes.Contains(clienteFora)))
                        {
                            rotaDb.DesativarClienteNaRota(rotaLocal, clienteForaRoteiro);
                        }
                    }

                    foreach (var clienteRotaERP in rotaERP.Clientes)
                    {
                        try
                        {
                            var clienteRota = clienteDb.FindByCodigo(clienteRotaERP.Codigo);
                            if (clienteRota == null) // Somente inclui na rota se tiver cliente cadastrado.
                            {
                                Logger.Warn(false,
                                            "Rota - Cliente \"{0}\" em \"{1}\" dia \"{2:dd/MM/yyyy}\" não encontrado",
                                            clienteRotaERP.Codigo, rotaLocal.Nome, rotaLocal.Dia);
                                continue;
                            }
                            clienteRota.OrdemRoteiro = clienteRotaERP.OrdemRoteiro;
                            clienteRota.IsAtivoRoteiro = clienteRotaERP.IsAtivoRoteiro;
                            clienteRota.IsPermitidoForaDia = clienteRotaERP.IsPermitidoForaDia;
                            if (!isRotaNova && rotaLocal.Clientes.FirstOrDefault(c => c.Equals(clienteRota)) != null)
                            // Já existe o cliente na rota.
                            {
                                // Se existe o cliente, e ele não está desativado na rota gravada local, deve atualizar
                                if (!rotaDb.VerificarDesabilitadoNaRota(rotaLocal, clienteRota))
                                {
                                    clienteRota.HasRota = true;
                                    clienteDb.Save(clienteRota);
                                    rotaDb.AtualizarClienteRota(clienteRota, rotaLocal);
                                    continue;
                                }
                            }
                            if (clienteRota.IsAtivoRoteiro) // Só inseri quando cliente está ativo no roteiro.
                            {
                                clienteRota.HasRota = true;
                                clienteDb.Save(clienteRota);
                                rotaDb.InserirClienteRota(clienteRota, rotaLocal);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(true, "Erro ao tratar o cliente {0} ordem {2} na rota {1}", clienteRotaERP.Codigo, rotaERP.Codigo, clienteRotaERP.OrdemRoteiro);
                            ExceptionPolicy.Handle(ex);
                        }
                    }

                    SalvarBlocoRota(dtosOnline.First(b => b.Bloco == BlocoStatusEnum.Inicial), dtosOnline.First(b => b.Bloco == BlocoStatusEnum.Final));
                    configuracao.IsIndiceInicialDiaModificado = false;
                    configuracaoDb.Save(configuracao);

                }
            }
            catch (Exception ex)
            {
                if (ExceptionPolicy.Handle(ex))
                {
                    throw;
                }
            }
        }

        private void SalvarBlocoRota(RotaDto rotaInicial, RotaDto rotaFinal)
        {
            var rotaDb = (IOfflineRotaDb)Offline;
            rotaInicial.Bloco = BlocoStatusEnum.Inicial;
            rotaFinal.Bloco = BlocoStatusEnum.Final;
            rotaDb.AtualizarBlocoStatus(rotaInicial);
            rotaDb.AtualizarBlocoStatus(rotaFinal);
        }

        /// <summary>
        /// Retorna as rotas com base nas pastas.
        /// </summary>
        /// <param name="pastas"></param>
        /// <param name="usuario"></param>
        /// <param name="indiceInicial">Indice que irá iniciar .</param>
        /// <param name="diaInicial"></param>
        /// <param name="ultimoIndice"></param>
        /// <returns></returns>
        private IEnumerable<RotaDto> GetRotasPastas(IEnumerable<Pasta> pastas, UsuarioDto usuario, int indiceInicial, DateTime diaInicial, bool ultimoIndice = false)
        {
            indiceInicial = indiceInicial == 0 ? 1 : indiceInicial;
            var rotas = new List<RotaDto>();
            var circular = new LinkedList<Pasta>(pastas.OrderBy(p => p.Indice));
            var pastaNode = circular.FindForCircular(pastas.First(p => p.Indice.Equals(indiceInicial)));
            int concatenaDia = 0;
            DateTime diaCalculo = diaInicial;
            pastaNode = ultimoIndice ? pastaNode.Proximo : pastaNode;
            for (int i = 0; i < circular.Count; i++)
            {
                var pasta = pastaNode.Value;
                var rota = new RotaDto
                {
                    Bloco = i == 0 ? // Atualiza o status conforme a rota for o inicio do bloco ou o final.
                             BlocoStatusEnum.Inicial : i == circular.Count - 1 ?
                                 BlocoStatusEnum.Final : BlocoStatusEnum.Indefinido,
                };
                rota.Codigo = pasta.Codigo;
                rota.Dia = diaCalculo;
                Logger.Debug("Parametros para calcular final de semana - rota.Dia.DayOfWeek {0} - ConcatenaDia {1} - rota.Dia.AddDays(concatenaDia).DayOfWeek: {2}",
                    rota.Dia.DayOfWeek, concatenaDia, rota.Dia.AddDays(concatenaDia).DayOfWeek);
                if ((rota.Dia.DayOfWeek == DayOfWeek.Saturday && concatenaDia == 0) || rota.Dia.AddDays(concatenaDia).DayOfWeek == DayOfWeek.Saturday)
                {
                    concatenaDia += 2;
                }
                else if ((rota.Dia.DayOfWeek == DayOfWeek.Sunday && concatenaDia == 0) || rota.Dia.AddDays(concatenaDia).DayOfWeek == DayOfWeek.Sunday)
                {
                    concatenaDia += 1;
                }
                rota.DiaPasta = rota.Dia;
                rota.Dia = rota.Dia.AddDays(concatenaDia);
                rota.Usuario = usuario;
                rota.Clientes = pasta.Clientes;
                rota.Nome = pasta.Nome;
                rota.IndicePasta = pasta.Indice;
                Logger.Debug("Adicionando pasta {0} - Dia Pasta {1} - Dia {2}", rota.IndicePasta, rota.DiaPasta, rota.Dia);
                rotas.Add(rota);

                diaCalculo = diaCalculo.AddDays(1);
                pastaNode = pastaNode.Proximo;
            }

            // Adicionar 5 dias para trás.
            var primeiroDia = rotas.First().Dia;
            var pastaAnteriorNode = pastaNode.Primeiro.Anterior;
            var diaAnterior = primeiroDia.AddDays(-1);
            int decrementaDia = 0;
            for (int i = 0; i < 5; i++)
            {
                var pasta = pastaAnteriorNode.Value;
                var rota = new RotaDto();
                rota.Codigo = pasta.Codigo;
                rota.Dia = diaAnterior;
                if ((rota.Dia.DayOfWeek == DayOfWeek.Sunday && decrementaDia == 0) || rota.Dia.AddDays(decrementaDia).DayOfWeek == DayOfWeek.Sunday)
                {
                    decrementaDia -= 2;
                }
                else if ((rota.Dia.DayOfWeek == DayOfWeek.Saturday && decrementaDia == 0) || rota.Dia.AddDays(decrementaDia).DayOfWeek == DayOfWeek.Saturday)
                {
                    decrementaDia -= 1;
                }
                rota.DiaPasta = rota.Dia;
                rota.Dia = rota.Dia.AddDays(decrementaDia);
                rota.Usuario = usuario;
                rota.Clientes = pasta.Clientes;
                rota.Nome = pasta.Nome;
                rota.IndicePasta = pasta.Indice;
                rotas.Insert(0, rota);

                pastaAnteriorNode = pastaAnteriorNode.Anterior;
                diaAnterior = diaAnterior.AddDays(-1);
            }

            return rotas;
        }

        private DateTime GetDateOfPasta(int indice, int totalPasta)
        {
            DateTime hoje = DateTime.Today;
            int diaMes = hoje.Day;
            if (diaMes > 0 && diaMes <= totalPasta)
            {
                return new DateTime(hoje.Year, hoje.Month, indice);
            }
            int totalAcumulado = totalPasta;
            for (int i = 1; totalAcumulado <= DateTime.DaysInMonth(hoje.Year, hoje.Month); i++)
            {
                totalAcumulado *= i;
                if (diaMes > totalAcumulado && diaMes <= (totalAcumulado * (i + 1)))
                {
                    return new DateTime(hoje.Year, hoje.Month, (totalAcumulado) + indice);
                }
            }
            return hoje;
        }
    }
}