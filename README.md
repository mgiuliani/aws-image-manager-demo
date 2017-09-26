# saws-image-manager

A simple site to demonstrate how to use the AWS .NET SDK to upload images to S3, store metadata in DynamoDB, and process that information in a server-less fashion using Lambda & Rekognitio



##Application Settings

###AwsImageManager/appsettings.json

| Setting                                | Usage                                    |
| -------------------------------------- | ---------------------------------------- |
| AWS.Profile                            | AWS credential profile                   |
| AWS.Region                             | The region to load resources within      |
| ImageManagerSettings.S3BucketName      | The name of the bucket that images will be uploaded into |
| ImageManagerSettings.DynamoDBTableName | The name of the DynamoDB table that will store image metadata |

 

##Deployment

###AwsImageManager

Deploy to Amazon Beanstalk.  The project is configured to run properly within an IIS environment.



*Beanstalk Instance Role Permissions*

- S3 Full Access

- DynamoDB Full Access

  ​

###AwsImageManager.Lambda

Deploy to Amazon Lambda, and create a trigger from the bucket defined in the `ImageManagerSettings.S3BucketName` application setting.



*Lambda Role Permissions*

- S3 Read Access
- DynamoDB Full Access
- Rekognition Read Access