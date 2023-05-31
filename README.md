Cubeflakes Insurance Claim Application
======================================

Welcome to the Cubeflakes Insurance Claim application! This demo application has been specifically developed to demonstrate the utilization of BoldSign APIs in the context of insurance claims. The demo is designed from the perspective of Cubeflakes, a fictional company, providing you with a practical example of how BoldSign can be integrated into real-world insurance processes.

##### BoldSign API key features used in this application:

*   Single Signer
*   Custom branding identity
*   Templated document with existing form fields
*   Auto filling form fields
*   Sign the document without leaving the website
*   Download signed document copy

##### Blog post for this application:

We have recently published a comprehensive blog post that details the workflow and implementation of the insurance claim demo application. This article focuses on the advantages of utilizing BoldSign eSignature APIs for insurance processes and offers guidance to developers on seamlessly integrating them into their applications. It delves into the diverse range of features and functionalities provided by BoldSign that can significantly enhance the insurance workflow, including secure document signing, automated document management, and customizable branding options.

Check out the blog post - [https://boldsign.com/blogs/integrating-esignatures-into-insurance-claim-app-using-boldsign-apis/](https://boldsign.com/blogs/integrating-esignatures-into-insurance-claim-app-using-boldsign-apis/)

##### Application code flow:

###### Step 1: Prerequisites

*   BoldSign account and an API key: The API key is located in the [API Key - BoldSign](https://app.boldsign.com/api-management/api-key/) page. Refer [BoldSign API documentation](https://developers.boldsign.com/authentication/api-key/) to generate and use API key from [BoldSign Account](https://account.boldsign.com/signup?planId=314).
*   BoldSign brand ID: Need to create a brand for our insurance company, which will provide a strong and consistent brand impression to the insurance claim users. A new branding can be created in the following two ways. Refer to the attached links for how to create a brand. One is using [BoldSign APIs](https://developers.boldsign.com/branding/create-brand/) and another one is using [BoldSign web application](https://support.boldsign.com/kb/article/19/customize-branding).
*   BoldSign template ID: we are creating a template with insurance claim form document. This allows to create claim document for different people without having to recreate the document each time. A new template can be created in following two ways. Refer to the attached links for how to create a template. One is using [BoldSign APIs](https://developers.boldsign.com/template/create-template/) and another one is using [BoldSign web application](https://support.boldsign.com/kb/article/31/create-template).

###### Step 2: Auto filling form fields

Upon submission of the form and completion of all mandatory fields in both the basic details and address sections, a document will be generated using an existing template from the BoldSign web application. The document will automatically populate the relevant fields with the user's inputs, ensuring seamless integration of the provided information.

###### Step 3: Get embedded signing link

After generating the auto-filled document from the existing template, the next step is to create an embed signing URL. This URL will be loaded into an iFrame within the application, allowing the user to complete the signing process without leaving the application.

Reference: [https://developers.boldsign.com/embedded-signing/get-embedded-signing-link/](https://developers.boldsign.com/embedded-signing/get-embedded-signing-link/)

###### Step 4: Download signed document

Once all the required fields have been filled in and the signing process is complete, you have the option to be redirected to a specified URL using the \`redirectUrl\` option. Additionally, if you wish to download a copy of the signed document for your records, you can utilize the following code snippet.

You may need to wait for the document to finish the signing process before creating a signed copy, and you cannot estimate the timing. At that moment, set up [webhooks](https://developers.boldsign.com/webhooks/introduction/) to listen for event triggers.

Reference: [https://developers.boldsign.com/embedded-signing/get-embedded-signing-link/](https://developers.boldsign.com/embedded-signing/get-embedded-signing-link/)

##### Other references:

*   BoldSign eSignature API: [https://developers.boldsign.com/](https://boldsign.com/esignature-api/)
*   BoldSign API documentation: [https://developers.boldsign.com/](https://developers.boldsign.com/)
*   BoldSign demos: [https://demos.boldsign.com/](https://demos.boldsign.com/)
