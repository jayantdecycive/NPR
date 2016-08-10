using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain.user;

namespace cfacore.shared.domain.user
{
    public class UserCollection : IUserCollection
    {
        public UserCollection() : base()
        { 
        
        }

        public UserCollection(List<User> Users)
            : base()
        {
            this._Users = Users;
        }

        private List<User> _Users = new List<User>();
        public int IndexOf(User item)
        {
            return _Users.IndexOf(item);
        }

        public void Insert(int index, User item)
        {
            _Users.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _Users.RemoveAt(index);
        }

        public User this[int index]
        {
            get
            {
                return _Users[index];
            }
            set
            {
                _Users[index] = value;
            }
        }

        public void Add(User item)
        {
            _Users.Add(item);
        }

        

        public void Add(int id)
        {
            _Users.Add(new User(id.ToString()));
        }

        public void Clear()
        {
            _Users.Clear();
        }

        public bool Contains(User item)
        {
            return _Users.Contains(item);
        }

        public void CopyTo(User[] array, int arrayIndex)
        {
            _Users.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Users.Count(); }
        }

        public bool IsReadOnly
        {
            get { 
                return false; 
            }
        }

        public bool Remove(User item)
        {
            return _Users.Remove(item);
        }

        public IEnumerator<User> GetEnumerator()
        {
            return _Users.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Users.GetEnumerator();
        }
    }
}
