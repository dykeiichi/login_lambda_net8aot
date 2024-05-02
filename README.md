# Login Lambda .NET 8 AOT

Project made to implement a secure way to manage user login and signup following best practices. It leverages AWS Lambda, AWS DynamoDB, and the performance benefits of .NET 8 AOT.

# Publish

To publish this project, you may need to compile it using a Docker image that includes .NET 8. For this purpose, I created this repository: https://github.com/dykeiichi/docker_dotnet_8_compiler_aws

1. Create a DynamoDB table with a String primary key named "Username" to store users.
2. Create a Lambda function.
3. Set an environment variable named "USERS_TABLE_NAME" that contains the user table name.
4. Grant the Lambda function permissions to access the DynamoDB table.
5. Configure the `aws-lambda-tools-defaults.json` file (e.g., specify region and profile).
6. Publish the code with:

```
dotnet-lambda deploy-function -ucfb true -cifb <Docker Image> --profile <AWS Lambda Profile>
```

**Note:** You can omit the "profile" tag if it's the default or already set in `aws-lambda-tools-defaults.json`.

7. Create an API Gateway as a "REST API" and publish it as a Proxy.