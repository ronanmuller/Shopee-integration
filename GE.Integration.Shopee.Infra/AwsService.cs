using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Net;

namespace GE.Integration.Core.Application.Services
{
    public interface IAwsService
    {
        Task<ReceiveMessageResponse> ReceiveAwsMessage(string receiveMessageQueueName);

        Task<HttpStatusCode> SendAwsMessage(string objSerialized, string sendMessageQueueName);

        Task<DeleteMessageResponse> DeleteAwsMessage(string receiptHandle, string queueUrl);

        string GetQueueUrlAsync(string queueName);

        Task<GetQueueAttributesResponse> GetQueueAttributesAsync(string queueUrl, List<string> attributeNames);

        int GetQueueApproximateNumberOfMessages(string queueUrl, List<string> attributeNames);
        Task<bool> DeleteMessageBatchByUrl(string queueUrl, List<Message> messages);
    }

    public class AwsService : IAwsService
    {
        private AmazonSQSClient _amazonSqsClient;
        public const string AWS_ERROR = "Erro ao acessar as configurações do AWS";

        public AwsService()
        {
            var awsSettings = "us-east-1";

            if (awsSettings == null)
                throw new Exception(AWS_ERROR);

            var region = RegionEndpoint.GetBySystemName(awsSettings);

            if (region == null)
                throw new Exception(AWS_ERROR);

            SetRegion(region);

            if (_amazonSqsClient == null)
                throw new Exception(AWS_ERROR);
        }

        public async Task<ReceiveMessageResponse> ReceiveAwsMessage(string receiveMessageQueueName)
        {
            ReceiveMessageResponse response;
            var request = new ReceiveMessageRequest
            {
                QueueUrl = receiveMessageQueueName
            };
            try
            {
                using (_amazonSqsClient)
                {
                    response = await _amazonSqsClient.ReceiveMessageAsync(request);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error when getting object message from SQS. Possible error is: - " + e.Message);
            }


            return response;
        }

        public async Task<HttpStatusCode> SendAwsMessage(string objSerialized, string sendMessageQueueName)
        {
          
            SendMessageResponse response;
            var request = new SendMessageRequest()
            {
                QueueUrl = sendMessageQueueName,
                MessageBody = objSerialized
            };

            try
            {
                using (_amazonSqsClient)
                {
                    response = await _amazonSqsClient.SendMessageAsync(request);
                }
            }
            catch (AmazonSQSException e)
            {
                Console.WriteLine($"SendAwsMessage {e.Message}");
                throw new Exception("Error when sending object message to SQS. Possible error is: - " + e.Message);
            }

            return response.HttpStatusCode;
        }

        public async Task<DeleteMessageResponse> DeleteAwsMessage(string receiptHandle, string queueUrl)
        {
        
            return await _amazonSqsClient.DeleteMessageAsync(queueUrl, receiptHandle);
        }

        public string GetQueueUrlAsync(string queueName)
        {
            return _amazonSqsClient.GetQueueUrlAsync(queueName).Result.QueueUrl;
        }

        public Task<GetQueueAttributesResponse> GetQueueAttributesAsync(string queueUrl, List<string> attributeNames)
        {
            return _amazonSqsClient.GetQueueAttributesAsync(queueUrl, attributeNames);
        }

        public int GetQueueApproximateNumberOfMessages(string queueUrl, List<string> attributeNames)
        {
            Task<GetQueueAttributesResponse> result = _amazonSqsClient.GetQueueAttributesAsync(
                queueUrl,
                new List<string> { QueueAttributeName.ApproximateNumberOfMessages, QueueAttributeName.ApproximateNumberOfMessagesNotVisible }
            );

            return result.Result.ApproximateNumberOfMessages + result.Result.ApproximateNumberOfMessagesNotVisible;
        }

        public async Task<bool> DeleteMessageBatchByUrl(string queueUrl, List<Message> messages)
        {
            var deleteRequest = new DeleteMessageBatchRequest()
            {
                QueueUrl = queueUrl,
                Entries = new List<DeleteMessageBatchRequestEntry>()
            };
            foreach (var message in messages)
            {
                deleteRequest.Entries.Add(new DeleteMessageBatchRequestEntry()
                {
                    ReceiptHandle = message.ReceiptHandle,
                    Id = message.MessageId
                });
            }

            var deleteResponse = await _amazonSqsClient.DeleteMessageBatchAsync(deleteRequest);

            return deleteResponse.Failed.Any();

        }

        #region Private Methods

        private void SetRegion(RegionEndpoint regionEndpoint)
        {
            try
            {
                _amazonSqsClient = new AmazonSQSClient(regionEndpoint ?? throw new ArgumentNullException(nameof(regionEndpoint)));
            }
            catch (Exception e)
            {
                _amazonSqsClient = null;
                throw new Exception(e.Message);
            }
        }

        #endregion
    }
}
