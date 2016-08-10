using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Xml;

namespace cfares.site.modules.com.datatables
{
    
    class DataTableRequestSupport : IDispatchMessageInspector
    {
        // Assume utf-8, note that Data Services supports
        // charset negotation, so this needs to be more
        // sophisticated (and per-request) if clients will 
        // use multiple charsets
        private static readonly Encoding encoding = Encoding.UTF8;

        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if (request.Properties.ContainsKey("UriTemplateMatchResults"))
            {
                HttpRequestMessageProperty httpmsg = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                UriTemplateMatch match = (UriTemplateMatch)request.Properties["UriTemplateMatchResults"];                

            }

            return null;
        }

        


        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            HttpResponseMessageProperty response = reply.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
            if (response != null)
            {
                string contentType = response.Headers["Content-Type"];
                if (contentType != null)
                {
                    // Check the response type and change it to text/javascript if we know how.
                    if (contentType.StartsWith("text/plain", StringComparison.InvariantCultureIgnoreCase))
                    {
                
                        response.Headers["Content-Type"] = "text/javascript;charset=utf-8";
                    }
                    else if (contentType.StartsWith("application/json", StringComparison.InvariantCultureIgnoreCase))
                    {
                        response.Headers["Content-Type"] = contentType.Replace("application/json", "text/javascript");
                    }
                }
            }

            XmlDictionaryReader reader = reply.GetReaderAtBodyContents();
            reader.ReadStartElement();

            string content = DataTableRequestSupport.encoding.GetString(reader.ReadContentAsBase64());
                

            Message newreply = Message.CreateMessage(MessageVersion.None, "", new Writer(content));
            newreply.Properties.CopyProperties(reply.Properties);

            reply = newreply;
           
        }

      

        #endregion

        class Writer : BodyWriter
        {
            private string content;

            public Writer(string content)
                : base(false)
            {
                this.content = content;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement("Binary");
                byte[] buffer = DataTableRequestSupport.encoding.GetBytes(this.content);
                writer.WriteBase64(buffer, 0, buffer.Length);
                writer.WriteEndElement();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DataTableRequestHandlerAttribute: Attribute, IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            Console.Write(bindingParameters);
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher ed in cd.Endpoints)
                {
                    ed.DispatchRuntime.MessageInspectors.Add(new DataTableRequestSupport());
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            
        }
    }
}
