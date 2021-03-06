﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace YieldForEachApp
{

    public interface IHyperloop<in T>
    {
        void Add(IEnumerable<T> source);
        void Add(IEnumerator<T> loop);
    }

    public sealed class Hyperloop<T>: IHyperloop<T>, IEnumerable<T>, /*IEnumerable,*/ IEnumerator<T> /*, IEnumerator, IDisposable */
    {

        private readonly Stack<IEnumerator<T>> _loops;

        #region Constructor

        public Hyperloop()
        {
            _loops = new Stack<IEnumerator<T>>();
        }

        public Hyperloop(int capacity)
        {
            _loops = new Stack<IEnumerator<T>>(capacity);
        }

        #endregion

        #region IHyperloop

        public void Add(IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Add(source.GetEnumerator());
        }

        public void Add(IEnumerator<T> loop)
        {
            if (loop == null)
                throw new ArgumentNullException(nameof(loop));

            _loops.Push(loop);
        }

        #endregion

        #region IEnumerable

        public IEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;

        #endregion

        #region IEnumerator

        public T Current => _loops.Peek().Current;

        object IEnumerator.Current => _loops.Peek().Current;

        public bool MoveNext()
        {
            while (_loops.Count > 0)
            {
                var workNode = _loops.Peek();
                if (workNode.MoveNext())
                {
                    if (workNode == _loops.Peek())
                        return true;
                }
                else
                    _loops.Pop().Dispose();
            }
            return false;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IDispose

        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }

        /*protected virtual*/ void Dispose(bool disposing)
        {
            if (!disposing) return;
            foreach (var node in _loops)
                node.Dispose();
            _loops.Clear();
        }

        #endregion
    }
}
