using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfares.domain.user
{
    public class ResUserCollection : IResUserCollection
    {

        public ResUserCollection() : base()
        { 
        
        }

        public ResUserCollection(List<ResUser> ResUsers)
            : base()
        {
            this._ResUsers = ResUsers;
        }

        public ResUserCollection(ICollection<ResUser> ResUsers)
            : base()
        {
            this._ResUsers = ResUsers.ToList();
        }

        private List<ResUser> _ResUsers = new List<ResUser>();
        public int IndexOf(ResUser item)
        {
            return _ResUsers.IndexOf(item);
        }

        public void Insert(int index, ResUser item)
        {
            _ResUsers.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _ResUsers.RemoveAt(index);
        }

        public ResUser this[int index]
        {
            get
            {
                return _ResUsers[index];
            }
            set
            {
                _ResUsers[index] = value;
            }
        }

        public void Add(ResUser item)
        {
            _ResUsers.Add(item);
        }

        public string JoinedIds() {
            if (this.Count == 0)
                return string.Empty;
            List<string> ids = new List<string>();
            foreach(ResUser u in this){
                if(u!=null)
                ids.Add(u.Id());
            }
            return string.Join(",", ids.ToArray());
        }

        public void Add(int id)
        {
            _ResUsers.Add(new ResUser(id.ToString()));
        }

        public void Clear()
        {
            _ResUsers.Clear();
        }

        public bool Contains(ResUser item)
        {
            return _ResUsers.Contains(item);
        }

        public void CopyTo(ResUser[] array, int arrayIndex)
        {
            _ResUsers.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _ResUsers.Count(); }
        }

        public bool IsReadOnly
        {
            get { 
                return false; 
            }
        }

        public bool Remove(ResUser item)
        {
            return _ResUsers.Remove(item);
        }

        public IEnumerator<ResUser> GetEnumerator()
        {
            return _ResUsers.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _ResUsers.GetEnumerator();
        }
    }
}
