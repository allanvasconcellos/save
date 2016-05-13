using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace INetSales.Objects
{
    public static class HelperLinkedList
    {
        public static CircularNode<T> FindForCircular<T>(this LinkedList<T> linkedList, T objToFind)
        {
            var node = linkedList.Find(objToFind);
            return new CircularNode<T>(node);
        }
    }

    public class CircularNode<T>
    {
        private readonly LinkedListNode<T> _beginNode;

        public CircularNode(LinkedListNode<T> beginNode)
        {
            _beginNode = beginNode;
        }

        public CircularNode<T> Primeiro
        {
            get
            {
                return new CircularNode<T>(_beginNode.List.First);
            }
        }

        public CircularNode<T> Proximo
        {
            get
            {
                var nextNode = _beginNode.Next ?? _beginNode.List.First;
                return new CircularNode<T>(nextNode);
            }
        }

        public CircularNode<T> Anterior
        {
            get
            {
                var previousNode = _beginNode.Previous ?? _beginNode.List.Last;
                return new CircularNode<T>(previousNode);
            }
        }

        public T Value
        {
            get { return _beginNode.Value; }
        }
    }
}