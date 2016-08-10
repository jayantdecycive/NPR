using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfacore.domain.card
{
    class AvailableSmartReward
    {
        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private System.DateTime _createDate = DateTime.Now;

        public System.DateTime createDate
        {
            get{ return this._createDate;}

            set{ this._createDate = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
        private bool _createDateSpecified = false;

        public bool createDateSpecified
        {
            get{ return this._createDateSpecified;}

            set{ this._createDateSpecified = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
        private System.DateTime _eligibleDate = DateTime.Now;

        public System.DateTime eligibleDate
        {
            get{ return this._eligibleDate;}

            set{ this._eligibleDate = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
        private bool _eligibleDateSpecified = false;

        public bool eligibleDateSpecified
        {
            get{ return this._eligibleDateSpecified;}

            set{ this._eligibleDateSpecified = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private int _rewardProgramId = 0;

        public int rewardProgramId
        {
            get{ return this._rewardProgramId;}

            set{ this._rewardProgramId = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private int _tierId = 0;

        public int tierId
        {
            get{ return this._tierId;}

            set{ this._tierId = value;}
        }

        /*
		* Generated code from the Core Model Builder Add-on
		Generated Tuesday, February 28, 2012 */
		private string _tierName = null;

        public string tierName
        {
            get{ return this._tierName;}

            set{ this._tierName = value;}
        }

       
        

    }
}
