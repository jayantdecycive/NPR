﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cfacome.entity.dao.Designer
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CfaComFoodEntities : DbContext
    {
        public CfaComFoodEntities()
            : base("name=CfaComFoodEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Food> Foods { get; set; }
        public DbSet<Food_Category> Food_Category { get; set; }
        public DbSet<Food_Variation> Food_Variation { get; set; }
    }
}