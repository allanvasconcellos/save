using System;
using System.Collections.Generic;
using System.Text;
using INetSales.Objects;
using System.Linq;

namespace INetSales.OfflineInterface.AndroidDb
{
	public class DtoMap
	{
		public const string CODIGO_COLUMN = "Codigo";
		public const string DATA_CRIACAO_COLUMN = "DataCriacao";
		public const string DATA_ALTERACAO_COLUMN = "DataAlteracao";
		public const string IS_DESABILITADO_COLUMN = "IsDesabilitado";
		public const string IS_PENDING_UPLOAD_COLUMN = "IsPendingUpload";
		public const string DATA_LAST_UPLOAD_COLUMN = "DataLastUpload";
	}

	public abstract class DtoMap<TDto>
		where TDto : IDto, new()
	{
		protected DtoMap(params string[] fields)
		{
			Fields = fields;
		}

		public string PrimaryKey { get; protected set; }
		public string TableName { get; protected set; }

		public string[] Fields { get; protected set; }

		public string GetTableName(string alias = null)
		{
			return String.Format(TableName + " {0}", alias ?? String.Empty);
		}

		public string GetQueryField(string alias = null)
		{
			return String.Join(",", Fields.Select(f => !String.IsNullOrEmpty(alias) ? String.Format("{0}.{1}", alias, f) : f));
		}

		protected Dictionary<string, object> GetCommonInsertMap(TDto dto, int nextPkValue)
		{
			var map = new Dictionary<string, object>();
			map.Add(PrimaryKey, nextPkValue);
			if(Fields.Contains(CODIGO_COLUMN)) map.Add(CODIGO_COLUMN, dto.Codigo);
			if (Fields.Contains(DATA_CRIACAO_COLUMN)) map.Add(DATA_CRIACAO_COLUMN, dto.DataCriacao);
			if (Fields.Contains(IS_DESABILITADO_COLUMN)) map.Add(IS_DESABILITADO_COLUMN, dto.IsDesabilitado);
			if (dto is IUploader)
			{
				var uploader = dto as IUploader;
				map.Add(IS_PENDING_UPLOAD_COLUMN, uploader.IsPendingUpload);
				map.Add(DATA_LAST_UPLOAD_COLUMN, uploader.DataLastUpload);
			}
			return map;
		}

		protected Dictionary<string, object> GetCommonUpdateMap(TDto dto)
		{
			var map = new Dictionary<string, object>();
			if (Fields.Contains(CODIGO_COLUMN)) map.Add(CODIGO_COLUMN, dto.Codigo);
			if (Fields.Contains(DATA_ALTERACAO_COLUMN)) map.Add(DATA_ALTERACAO_COLUMN, dto.DataAlteracao);
			if (Fields.Contains(IS_DESABILITADO_COLUMN)) map.Add(IS_DESABILITADO_COLUMN, dto.IsDesabilitado);
			if (dto is IUploader)
			{
				var uploader = dto as IUploader;
				map.Add(IS_PENDING_UPLOAD_COLUMN, uploader.IsPendingUpload);
				map.Add(DATA_LAST_UPLOAD_COLUMN, uploader.DataLastUpload);
			}
			return map;
		}

		public bool Map(SqlReader reader, TDto dto)
		{
			int idIndex = reader.GetColumnIndex(PrimaryKey);
			int codigoIndex = reader.GetColumnIndex(CODIGO_COLUMN);
			int dataCriacaoIndex = reader.GetColumnIndex(DATA_CRIACAO_COLUMN);
			int dataAlteracaoIndex = reader.GetColumnIndex(DATA_ALTERACAO_COLUMN);
			int isDesabilitadoIndex = reader.GetColumnIndex(IS_DESABILITADO_COLUMN);
			int isPendingUploadIndex = reader.GetColumnIndex(IS_PENDING_UPLOAD_COLUMN);
			int dataLastUploadIndex = reader.GetColumnIndex(DATA_LAST_UPLOAD_COLUMN);
			//Logger.Debug("Table: {2} Index: {0} ID: {1}", idIndex, reader.GetInt(idIndex), TableName);
			dto.Id = reader.GetInt(idIndex);
			dto.Codigo = reader.GetValueOrDefault<string>(codigoIndex);
			dto.DataCriacao = reader.GetValueOrDefault<DateTime>(dataCriacaoIndex);
			dto.DataAlteracao = reader.GetValueOrDefault<DateTime?>(dataAlteracaoIndex);
			dto.IsDesabilitado = reader.GetBool(isDesabilitadoIndex);
			if (dto is IUploader)
			{
				var uploader = dto as IUploader;
				uploader.IsPendingUpload = reader.GetBool(isPendingUploadIndex);
				uploader.DataLastUpload = reader.GetValueOrDefault<DateTime?>(dataLastUploadIndex);
			}
			return DoEspecificMap(reader, dto);
		}

		public abstract IDictionary<string, object> GetInsertMap(TDto dto, out int id);

		public abstract IDictionary<string, object> GetUpdateMap(TDto dto);

		public abstract IDictionary<string, object> GetKeyMap();

		protected abstract bool DoEspecificMap(SqlReader reader, TDto dto);
	}
}

