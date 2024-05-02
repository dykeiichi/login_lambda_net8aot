using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using LoginLambda.Models;
using Shared.DataAccess;

namespace LoginLambda.Shared.DataAccess
{
    public class DynamoDbUsers : IUsersDAO
    {
        private static readonly string USERS_TABLE_NAME = Environment.GetEnvironmentVariable("USERS_TABLE_NAME") ?? "";
        private readonly AmazonDynamoDBClient _dynamoDbClient;

        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DynamoDbUsers))]
        public DynamoDbUsers()
        {
            this._dynamoDbClient = new AmazonDynamoDBClient();
            this._dynamoDbClient.DescribeTableAsync(USERS_TABLE_NAME).GetAwaiter().GetResult();
        }

        public UserContext? GetUser(string Username)
        {
            Task<GetItemResponse>? getItemResponse = this._dynamoDbClient.GetItemAsync(new GetItemRequest(USERS_TABLE_NAME,
                new Dictionary<string, AttributeValue>(1)
                {
                    {UserMapper.PK, new AttributeValue(Username)}
                }));

            if (getItemResponse.Wait(30000)) {
                return getItemResponse.Result.IsItemSet ? UserMapper.UserFromDynamoDB(getItemResponse.Result.Item) : null;
            } else {
                return null;
            }
        }

        public async Task PutUser(UserContext user)
        {
            await this._dynamoDbClient.PutItemAsync(USERS_TABLE_NAME, UserMapper.UserToDynamoDb(user));
        }

        public async Task DeleteUser(string Username)
        {
            await this._dynamoDbClient.DeleteItemAsync(USERS_TABLE_NAME, new Dictionary<string, AttributeValue>(1)
            {
                {UserMapper.PK, new AttributeValue(Username)}
            });
        }

        public async Task<UserWrapper> GetAllUsers()
        {
            var data = await this._dynamoDbClient.ScanAsync(new ScanRequest()
            {
                TableName = USERS_TABLE_NAME,
                Limit = 20
            });

            var users = new List<UserContext>();

            foreach (var item in data.Items)
            {
                users.Add(UserMapper.UserFromDynamoDB(item));
            }

            return new UserWrapper(users);
        }
    }
}