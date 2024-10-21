using Amazon.SQS.Model;
using System.Net;

namespace GE.Integration.Shopee.Application.Aws
{
    public interface IAwsService
    {
        Task<ReceiveMessageResponse?> ReceiveAwsMessage(string receiveMessageQueueName);

        Task<HttpStatusCode> SendAwsMessage(string objSerialized, string sendMessageQueueName);

        Task<DeleteMessageResponse> DeleteAwsMessage(string receiptHandle, string queueUrl);
        
        string GetQueueUrlAsync(string queueName);

        Task<GetQueueAttributesResponse> GetQueueAttributesAsync(string queueUrl, List<string> attributeNames);

        int GetQueueApproximateNumberOfMessages(string queueUrl, List<string> attributeNames);

        Task<bool> DeleteMessageBatchByUrl(string queueUrl, List<Message> messages);
    }
}
