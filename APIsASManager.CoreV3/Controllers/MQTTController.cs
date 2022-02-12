using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apache.NMS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace APIsASManager.CoreV3.Controllers
{
    [Route("v{version:apiVersion}/api/[controller]/[action]")]
    [ApiController]
    //[AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiVersion("3.0")]
    public class MQTTController : ControllerBase
    {
        IConfiguration configuration;
        public MQTTController(IConfiguration pConfig)
        {
            configuration = pConfig;
        }
        //[HttpPost]
        //public async Task ConnectAsync()
        //{
        //    //string clientId = Guid.NewGuid().ToString();
        //    string clientId = "apiClient";
        //    string mqttURI = configuration["MqttOption:HostIp"].ToString();
        //    string mqttUser = configuration["MqttOption:UserName"].ToString();
        //    string mqttPassword = configuration["MqttOption:Password"].ToString();
        //    int mqttPort = Convert.ToInt32(configuration["MqttOption:HostPort"].ToString());
        //    bool mqttSecure = false;

        //    var messageBuilder = new MqttClientOptionsBuilder()
        //    .WithClientId(clientId)
        //    .WithCredentials(mqttUser, mqttPassword)
        //    .WithTcpServer(mqttURI, mqttPort)
        //    .WithCleanSession();

        //    var options = mqttSecure
        //      ? messageBuilder
        //        .WithTls()
        //        .Build()
        //      : messageBuilder
        //        .Build();

        //    var managedOptions = new ManagedMqttClientOptionsBuilder()
        //      .WithAutoReconnectDelay(TimeSpan.FromSeconds(10))
        //      .WithClientOptions(options)
        //      .Build();

        //    client = new MqttFactory().CreateManagedMqttClient();

        //    await client.StartAsync(managedOptions);
        //}
        //[HttpPost]
        //public async Task PublishAsync(MyClass.ImageJson imageJson, bool retainFlag = true, int qos = 1)
        //{
        //    imageJson = new MyClass.ImageJson();
        //    string payload = JsonConvert.SerializeObject(imageJson);

        //    await client.PublishAsync(new MqttApplicationMessageBuilder()
        //      .WithTopic("image/mq/topic/" + imageJson.deviceId)
        //      .WithPayload(payload)
        //      .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
        //      .WithRetainFlag(retainFlag)
        //      .Build());
        //}
        [HttpPost]
        public async Task SendNewMessage(MyClass.ImageJson imageJson)
        {
            string clientId = "apiClient";
            string mqttURI = configuration["MqttOption:HostIp"].ToString();
            string mqttUser = configuration["MqttOption:UserName"].ToString();
            string mqttPassword = configuration["MqttOption:Password"].ToString();
            int mqttPort = Convert.ToInt32(configuration["MqttOption:HostPort"].ToString());
            bool mqttSecure = false;

            imageJson = new MyClass.ImageJson();
            string topic = $"image/mq/topic/" + imageJson.deviceId;

            Uri brokerUri = new Uri($"tcp://" + mqttURI + ":" + mqttPort.ToString());  // Default port
            IConnectionFactory factory = new Apache.NMS.ActiveMQ.ConnectionFactory(brokerUri);
            using (IConnection connection = factory.CreateConnection())
            {
                connection.ClientId = "apiClient";
                connection.Start();

                using (Apache.NMS.ISession session = connection.CreateSession())
                {
                    Apache.NMS.ActiveMQ.Commands.ActiveMQTopic activeMQTopic = new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(topic);
                    IMessageProducer prod = session.CreateProducer(activeMQTopic);
                    ITextMessage msg = prod.CreateTextMessage();
                    msg.NMSCorrelationID = clientId;
                    msg.Text = JsonConvert.SerializeObject(imageJson);
                    prod.Send(msg, MsgDeliveryMode.Persistent, MsgPriority.Normal, TimeSpan.MinValue);
                }
                //connection.Stop();
            }

            //using (IConnection connection = factory.CreateConnection())
            //{
            //    connection.ClientId = "apiClient";
            //    Apache.NMS.ISession session = connection.CreateSession();
            //    IDestination destination = Apache.NMS.Util.SessionUtil.GetDestination(session, "topic://" + topic);
            //    IMessageProducer producer = session.CreateProducer(destination);

            //    connection.Start();
            //    producer.DeliveryMode = MsgDeliveryMode.Persistent;
            //    producer.RequestTimeout = TimeSpan.FromSeconds(10);
            //    ITextMessage request = session.CreateTextMessage(JsonConvert.SerializeObject(imageJson));
            //    request.NMSCorrelationID = "abc";
            //    producer.Send(request);
            //    connection.Stop();
            //}
        }
    }
}
