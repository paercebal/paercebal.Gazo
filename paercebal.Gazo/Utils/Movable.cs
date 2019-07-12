using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paercebal.Gazo.Utils
{
    public class Movable<T>
        : IDisposable
        where T: class, IDisposable
    {
        private T resource;

        public Movable()
        {
        }

        public Movable(T resource_)
        {
            this.resource = resource_;
        }

        public Movable(Movable<T> that)
        {
            this.resource = that.Release();
        }

        public T Get()
        {
            return this.resource;
        }

        public void Set(T resource_)
        {
            this.resource.Dispose();
            this.resource = resource_;
        }

        public T Release()
        {
            T temp = this.resource;
            this.resource = null;
            return temp;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(this.resource != null)
                {
                    this.resource.Dispose();
                    this.resource = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
