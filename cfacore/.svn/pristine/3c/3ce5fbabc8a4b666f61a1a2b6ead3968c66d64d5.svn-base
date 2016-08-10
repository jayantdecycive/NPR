using System;
using System.Collections.Generic;
using cfacore.shared.domain._base;
namespace cfacore.site.controllers._base
{
    public delegate void LoadEventHandler(object sender, DomainServiceEventArgs e);
    public delegate void SaveEventHandler(object sender, DomainServiceEventArgs e);
    public delegate void DeleteEventHandler(object sender, DomainServiceEventArgs e);
    public delegate void SearchEventHandler(object sender, DomainServiceEventArgs e);

    public interface IDomainService<IDomainObject>
    {
        IDomainObject[] Search(KeyValuePair<string, string>[] criteria);
        IDomainObject Load(string ID);
        IDomainObject Load(Uri uri);
        bool Save(IDomainObject obj);
        bool Delete(IDomainObject obj);
        List<IDomainObject> GetAll();

        void OnSearch(DomainServiceEventArgs e);
        void OnLoad(DomainServiceEventArgs e);
        void OnSave(DomainServiceEventArgs e);
        void OnDelete(DomainServiceEventArgs e);

        event LoadEventHandler Loaded;
        event SaveEventHandler Saved;
        event DeleteEventHandler Deleted;
    }
}
