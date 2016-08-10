


DomainModel.MallStores = DomainModel.ResStoreCollection.extend({ query: { "manager": { ConceptCode: 1 } } });

//DomainModel.ActiveEvents = DomainModel.ResEventCollection.extend({ query: { "manager": { Status: 'Live' } } });
DomainModel.ActiveEvents = DomainModel.ResEventCollection.extend();
