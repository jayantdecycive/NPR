using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

using System.Web.Services;

using cfacore.wse.aloha.dao._base;
using System.ServiceModel;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security.Tokens;
using Microsoft.Web.Services3.Design;

using System.Web;
using cfacore.wse.aloha.dao.com.alohaenterprise.memberlink;

namespace cfacore.wse.aloha.dao
{
    public class CardService : AlohaService
    {
#if DEBUG
        public CardService(bool production = false)
            : base(production)
        {
        }
#else
        public CardService(bool production = true) 
            : base(production)
        {
        }
#endif

        public bool EmailExists(string email) {
            
            try
            {
                eMailExistsRequest gcsReq = new eMailExistsRequest();
                SetRequestCredential(gcsReq);                
                gcsReq.eMailAddress = email; 
                EMailExistsResult gcsResp = mlws.eMailExists(gcsReq);
                Console.WriteLine("\nGetCardStatusRequest Status: (" + gcsResp.executionStatus + ") "
            + gcsResp.executionStatusDescription);
                return gcsResp.EMailAddressExists;
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }

            return false;

            
        }
        public CardService()
            : base()
        {
            
        }

        public void TestApi() {


            UsernameToken token = new UsernameToken(wrapUserID, wrapPassword, PasswordOption.SendPlainText);

			mlws.SetClientCredential(token);
			mlws.SetPolicy("ClientPolicy");

            try
            {
                GetCardStatusRequest gcsReq = new GetCardStatusRequest();
                gcsReq.userID = userID;
                gcsReq.password = password;
                gcsReq.companyID = companyID;
                gcsReq.requestTime = DateTime.Now;
                gcsReq.cardNumber = "81000000000001"; //You will need to enter an existing card number that resides in the database here
                GetCardStatusResult gcsResp = mlws.getCardStatus(gcsReq);
                
                Console.WriteLine("\nGetCardStatusRequest Status: (" + gcsResp.executionStatus + ") "
            + gcsResp.executionStatusDescription);

                GetBonusPlanStandingsRequest gbsReq = new GetBonusPlanStandingsRequest();
                gbsReq.userID = userID;
                gbsReq.password = password;
                gbsReq.companyID = companyID;
                gbsReq.requestTime = DateTime.Now;
                GetBonusPlanStandingsResult gbsResp = mlws.getBonusPlanStandings(gbsReq);
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }
        }

        public int GetNewCardBatchID()
        {
            GetNewCardBatchIDResult result = null;
            try
            {
                GetNewCardBatchIDRequest request = new GetNewCardBatchIDRequest();
                SetRequestCredential(request);
                result = mlws.getNewCardBatchID(request);

                Console.WriteLine("\nGetNewCardBatchIDRequest Status: (" + result.executionStatus + ") "
                    + result.executionStatusDescription);
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }

            return result.batchID;
        }

        public CreateNewCardResult CreateNewCard(string cardNumber)
        {
            CreateNewCardResult result = null;
            try
            {
                CreateNewCardRequest req = new CreateNewCardRequest();
                SetRequestCredential(req);

                req.batchID = GetNewCardBatchID();
                req.batchDesc = DateTime.Now.ToString();
                req.cardSeries = "81018";
                req.numberOfCards = 1;
                req.startingCardNumber = cardNumber;
                req.numericSequenceType = 1;
                req.activateCard = true;

                result = mlws.createNewCard(req);

                Console.WriteLine("\nCreateNewCardRequest Status: (" + result.executionStatus + ") "
                    + result.executionStatusDescription);
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }

            return result;
        }

        //public DeactivateNewCardResult DeactivateCard(string cardNumber)
        //{
        //    DeactivateNewCardResult result = null;
        //    try
        //    {
        //        DeactivateNewCardRequest request = new DeactivateNewCardRequest();
        //        SetRequestCredential(request);

        //        result = mlws.deactivateNewCard(request);

        //        Console.WriteLine("\nGetCardStatusRequest Status: (" + result.executionStatus + ") "
        //            + result.executionStatusDescription);

        //    }
        //    catch (System.Web.Services.Protocols.SoapException e)
        //    {
        //        Console.WriteLine("exception=" + e);
        //        Console.WriteLine("exception=" + e.InnerException);
        //        Console.WriteLine("exception=" + e.Message);
        //    }

        //    return result;
        //}

        public GetCardStatusResult GetCardStatus(string cardNumber)
        {
            
            GetCardStatusResult gcsResp = null;
            try
            {
                GetCardStatusRequest gcsReq = new GetCardStatusRequest();
                SetRequestCredential(gcsReq);
                gcsReq.cardNumber = cardNumber; //You will need to enter an existing card number that resides in the database here
                
                gcsResp = mlws.getCardStatus(gcsReq);
                
                Console.WriteLine("\nGetCardStatusRequest Status: (" + gcsResp.executionStatus + ") "
            + gcsResp.executionStatusDescription);

            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }
            return gcsResp;

        }

        public AlohaExecution Save(MemberProfile member) {

            if (GetCardStatus(member.cardNumber).profileExists)
            {
                return new AlohaExecution(UpdateMember(member));
            }
            else {
                return new AlohaExecution(AddMember(member));
            }

        }

        public AddMemberProfileResult AddMember(MemberProfile member)
        {            
            
            AddMemberProfileResult addResp = null;
            try
            {
                AddMemberProfileRequest addReq = new AddMemberProfileRequest();
                SetRequestCredential(addReq);
                //member.companyDefined10 = "True";
                addReq.profile = member;
                addResp = mlws.addMemberProfile(addReq);                

            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }
            return addResp;
        }

        public UpdateMemberProfileResult UpdateMember(MemberProfile member)
        {
            UpdateMemberProfileResult upResult = null;
            try
            {
                UpdateMemberProfileRequest gcsReq = new UpdateMemberProfileRequest();
                SetRequestCredential(gcsReq);
                //member.companyDefined10 = "True";
                gcsReq.profile = member;
                upResult = mlws.updateMemberProfile(gcsReq);                

            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }
            return upResult;
        }

        public GetMemberProfileResult GetMemberByCard(string cardNumber)
        {

            GetMemberProfileResult gcsResp = null;
            try
            {
                GetMemberProfileRequest gcsReq = new GetMemberProfileRequest();
                SetRequestCredential(gcsReq);
                gcsReq.cardNumber = cardNumber; //You will need to enter an existing card number that resides in the database here
                gcsResp = mlws.getMemberProfile(gcsReq);
                Console.WriteLine("\nGetCardStatusRequest Status: (" + gcsResp.executionStatus + ") "
            + gcsResp.executionStatusDescription);

            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }
            return gcsResp;

        }

       
        public LoginResult GetMemberByCard(string cardno, string password)
        {
            LoginResult res = new LoginResult();
            

            GetMemberProfileResult gcsResp = null;
            
            GetMemberProfileRequest gcsReq = new GetMemberProfileRequest();
            SetRequestCredential(gcsReq);
            gcsReq.cardNumber = cardno;
            gcsResp = mlws.getMemberProfile(gcsReq);
            Console.WriteLine("\nGetCardStatusRequest Status: (" + gcsResp.executionStatus + ") "
        + gcsResp.executionStatusDescription);
         

            if (gcsResp.profile==null||!gcsResp.profile.profileExists) {
                res.Message = "Profile does not exist";
                return res;
            }

            if (gcsResp.profile.companyDefined6 != password.Trim())
            {
                res.Message = "Password is incorrect";
                res.Profile = null;
                return res;
            }
            res.Profile = gcsResp.profile;
            return res;

        }

        private MemberProfile _CurrentProfile = null;
        public MemberProfile CurrentProfile {
            get {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                    return null;
                if (_CurrentProfile == null)
                    _CurrentProfile = GetMemberByCard(HttpContext.Current.User.Identity.Name).profile;
                return _CurrentProfile;
            }
        }

        public bool OptIn(string cardNumber)
        {
            OptInResult result = null;
            try
            {
                OptInRequest request = new OptInRequest();
                SetRequestCredential(request);
                request.cardNumber = cardNumber;

                result = mlws.optIn(request);

                Console.WriteLine("\nOptInResult Status: (" + result.executionStatus + ") "
                    + result.executionStatusDescription);
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }

            return (result.executionStatus == 0);
        }

        public OptOutResult OptOut(string cardNumber)
        {
            OptOutResult result = null;
            try
            {
                OptOutRequest request = new OptOutRequest();
                SetRequestCredential(request);
                request.cardNumber = cardNumber;

                result = mlws.optOut(request);

                Console.WriteLine("\nOptOutResult Status: (" + result.executionStatus + ") "
                    + result.executionStatusDescription);
            }   
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }

            return result;
        }

        public string Password(MemberProfile mem) {
            return (string)mem.companyDefined6;
        }

        public void Password(MemberProfile mem,string password)
        {
            mem.companyDefined6=password;
        }

        

        public EMailExistsResult EmailExistsReq(string email)
        {
            
            EMailExistsResult gcsResp = null;
            try
            {
                eMailExistsRequest gcsReq = new eMailExistsRequest();
                SetRequestCredential(gcsReq);
                gcsReq.eMailAddress = email;
                //gcsReq.cardNumber = cardNumber; //You will need to enter an existing card number that resides in the database here
                gcsResp = mlws.eMailExists(gcsReq);
                Console.WriteLine("\nGetCardStatusRequest Status: (" + gcsResp.executionStatus + ") "
            + gcsResp.executionStatusDescription);

            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }
            
            return gcsResp;

        }

        /// <summary>
        /// Validates passcode
        /// </summary>
        /// <param name="memberProfile"></param>
        /// <param name="passCode"></param>
        /// <returns></returns>
        public ValidatePassCodeResult ValidatePasscode(string cardNumber, string passCode)
        {
            ValidatePassCodeResult rsp = null;
            try
            {
                // Validate passcode
                ValidatePassCodeRequest request = new ValidatePassCodeRequest();
                SetRequestCredential(request);
                request.cardNumber = cardNumber;
                request.passCode = passCode;

                rsp = mlws.validatePassCode(request);

                Console.WriteLine("\nValidatePassCodeResult Status: (" + rsp.executionStatus + ") "
                    + rsp.executionStatusDescription);
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }

            return rsp;
        }

        public assignment[] GetBonusPlanHistory(string cardNumber)
        {
            GetBonusPlanHistoryResult result = null;
            try
            {
                GetBonusPlanHistoryRequest request = new GetBonusPlanHistoryRequest();
                SetRequestCredential(request);
                request.cardNumber = cardNumber;

                result = mlws.getBonusPlanHistory(request);

                Console.WriteLine("\nGetBonusPlanHistoryResult Status: (" + result.executionStatus + ") "
                    + result.executionStatusDescription);
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }

            return result.assignment;
        }

        public GetBonusPlanStandingsResult GetBonusPlanStanding(string cardNumber)
        {
            GetBonusPlanStandingsResult result = null;
            try
            {
                GetBonusPlanStandingsRequest request = new GetBonusPlanStandingsRequest();
                SetRequestCredential(request);
                request.cardNumber = cardNumber;

                result = mlws.getBonusPlanStandings(request);

                Console.WriteLine("\nGetBonusPlanStandingsResult Status: (" + result.executionStatus + ") "
                    + result.executionStatusDescription);
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }

            return result;
        }

        public AdjustCreditResult AdjustCredit(string CardNumber, int BPID, int AdjustmentType, double BPCredit, string Reason)
        {
            AdjustCreditResult result = null;
            try
            {
                AdjustCreditRequest request = new AdjustCreditRequest();
                SetRequestCredential(request);
                request.cardNumber = CardNumber;
                request.BPID = BPID;
                request.adjustmentType = AdjustmentType;
                request.BPCredit = BPCredit;
                request.reason = Reason;

                result = mlws.adjustCredit(request);

                Console.WriteLine("\nGetBonusPlanStandingsResult Status: (" + result.executionStatus + ") "
                    + result.executionStatusDescription);
            }
            catch (System.Web.Services.Protocols.SoapException e)
            {
                Console.WriteLine("exception=" + e);
                Console.WriteLine("exception=" + e.InnerException);
                Console.WriteLine("exception=" + e.Message);
            }

            return result;
        }
    }
    
}
