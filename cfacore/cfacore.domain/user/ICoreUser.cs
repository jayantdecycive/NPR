using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Text;
using cfacore.domain.store;
using core.synchronization.Automation;
using cfacore.domain.card;
using cfacore.shared.domain.user;

namespace cfacore.domain.user
{
    [ITable(Name="User")]
    public interface ICoreUser : IUser
    {
        /// <summary>
        /// Gets or sets the applications.
        /// </summary>
        /// <value>
        /// The applications.
        /// </value>
        IApplicationCollection Applications
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cards.
        /// </summary>
        /// <value>
        /// The cards.
        /// </value>
        ICardCollection Cards
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the stores.
        /// </summary>
        /// <value>
        /// The stores.
        /// </value>
        IStoreCollection Stores
        {
            get;
            set;
        }

        [Association(Name = "FK_Home", ThisKey = "HomeId", OtherKey = "HomeId")]
        IHome Home
        {
            get;
            set;
        }
    }
}
