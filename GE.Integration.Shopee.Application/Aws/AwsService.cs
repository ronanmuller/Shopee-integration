using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Net;

namespace GE.Integration.Shopee.Application.Aws
{
    public class AwsService : IAwsService
    {
        public AmazonSQSClient AmazonSqsClient { get; private set; }
  
        public async Task<ReceiveMessageResponse?> ReceiveAwsMessage(string receiveMessageQueueName)
        {
            ReceiveMessageResponse? response;

            var request = new ReceiveMessageRequest
            {
                QueueUrl = receiveMessageQueueName
            };

            using var client = SetAwsClient();
            response = await client.ReceiveMessageAsync(request);

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

            using var client = SetAwsClient();
            response = await client.SendMessageAsync(request);

            return response.HttpStatusCode;
        }

        public async Task<DeleteMessageResponse> DeleteAwsMessage(string receiptHandle, string queueUrl)
        {
            using var client = SetAwsClient();
            return await client.DeleteMessageAsync(queueUrl, receiptHandle);
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

            using var client = SetAwsClient();
            var deleteResponse = await client.DeleteMessageBatchAsync(deleteRequest);

            return deleteResponse.Failed.Any();

        }

        public string GetQueueUrlAsync(string queueName)
        {
            using var client = SetAwsClient();
            return client.GetQueueUrlAsync(queueName).Result.QueueUrl;
        }

        public async Task<GetQueueAttributesResponse> GetQueueAttributesAsync(string queueUrl, List<string> attributeNames)
        {
            using var client = SetAwsClient();
            return await client.GetQueueAttributesAsync(queueUrl, attributeNames);
        }

        public int GetQueueApproximateNumberOfMessages(string queueUrl, List<string> attributeNames)
        {
            Task<GetQueueAttributesResponse> result;
            using var client = SetAwsClient();

            result = client.GetQueueAttributesAsync(
                queueUrl,
                new List<string>
                {
                        QueueAttributeName.ApproximateNumberOfMessages,
                        QueueAttributeName.ApproximateNumberOfMessagesNotVisible
                }
            );

            return result.Result.ApproximateNumberOfMessages + result.Result.ApproximateNumberOfMessagesNotVisible;
        }

        #region Private Methods

        private AmazonSQSClient SetAwsClient()
        {
            var region = RegionEndpoint.USEast1;

            if (region == null)
                throw new Exception("Erro ao criar o CLient AWS");

            return AmazonSqsClient = new AmazonSQSClient(region);

        }

        #endregion
    }
}
