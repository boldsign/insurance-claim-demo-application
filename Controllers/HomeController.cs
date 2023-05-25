
using BoldSign.Api;
using BoldSign.Api.Model;
using BoldSign.Model;
using InsuranceDemo.Models;
using Microsoft.AspNetCore.Mvc;
using BoldSign.Model.Webhook;
using Microsoft.Extensions.Caching.Distributed;

namespace InsuranceDemo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static readonly ApiClient apiClient = new ApiClient("https://api.boldsign.com",Environment.GetEnvironmentVariable("APIKEY"));
    private readonly string templateId = Environment.GetEnvironmentVariable("TEMPLATEID");
    private readonly DocumentClient documentClient = new DocumentClient(apiClient);
    private readonly TemplateClient templateClient = new TemplateClient(apiClient);
    private readonly IDistributedCache _cache;
    
    public HomeController(ILogger<HomeController> logger, IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }
    
    [HttpGet("home/getStatus/{id}")]
    public async Task<IActionResult> GetStatus(string id)
    {
        // Check if the document exists in the cache and its status is "completed"
        var status = await _cache.GetStringAsync(id);
        if (status == null || status != "completed")
        {
            // Document does not exist in the cache or its status is not "completed"
            return NotFound();
        }

        // Document has been completed and is ready for download
        return Ok();
    }
    
    [HttpPost("Home/Webhook")]
    [IgnoreAntiforgeryToken]
    // Action for Webhook
    public async Task<IActionResult> Webhook()
    {
        var sr = new StreamReader(this.Request.Body);
        var json = await sr.ReadToEndAsync();
        
        if (this.Request.Headers[WebhookUtility.BoldSignEventHeader] == "Verification")
        {
            return this.Ok();
        }

        // TODO: Update your webhook secret key
        var SECRET_KEY = Environment.GetEnvironmentVariable("WEBHOOKKEY");

        try
        {
            WebhookUtility.ValidateSignature(json, this.Request.Headers[WebhookUtility.BoldSignSignatureHeader], SECRET_KEY);
        }
        catch (BoldSignSignatureException ex)
        {
            Console.WriteLine(ex);

            return this.Forbid();
        }

        var eventPayload = WebhookUtility.ParseEvent(json);
        var doc = eventPayload.Data as DocumentEvent;
        if (eventPayload.Event.EventType == WebHookEventType.Completed)
        {
            Console.WriteLine("Signing process completed");
            // Store the results in the cache with the same document ID
            _cache.SetString(doc.DocumentId, "completed", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            // Return the ID of the document to the client
            return RedirectToAction("SignCompleted", new { doc.DocumentId });
        }
        return this.Ok();
    }

    // Action for ClaimForm Basic Details
    public IActionResult ClaimFormBasicDetails([FromRoute] string id)
    {
        var storageModel = HttpContext.Items["TemplateDetails"] as TemplateDetails;
        var templateId = id.Substring(0, 1).ToUpper() + id.Substring(1);;
        if (storageModel != null)
        {
            storageModel.PolicyType = templateId;
        }
        var templateDetails = new TemplateDetails();
        templateDetails.PolicyType = templateId;
        return View(templateDetails);
    }
    
    // Action for ClaimForm Address Information
    public IActionResult ClaimFormAddressInformation(TemplateDetails templateDetails)
    {
        // Store basic details as template details object
        var storageModel = HttpContext.Items["TemplateDetails"] as TemplateDetails;
        if (storageModel != null)
        {
            storageModel.FullName=  templateDetails.FullName;
            storageModel.Gender = templateDetails.Gender;
            storageModel.DOB= templateDetails.DOB;
            storageModel.PolicyNumber= templateDetails.PolicyNumber;
            storageModel.DamageDetails= templateDetails.DamageDetails;
            storageModel.InsuredObject = storageModel.PolicyType.ToUpper();
        }
        return View(templateDetails);
    }
    
    // Action for ClaimForm Signing Completed
    public IActionResult SignCompleted()
    {
        return View();
    }

    // Action for ClaimForm Home Page
    public IActionResult Index()
    {
        return View();
    }

    // Download the document using DocumentId
    [HttpGet]
    public async Task<IActionResult> DownloadDocument()
    {
        var storageModel = HttpContext.Items["TemplateDetails"] as TemplateDetails;
        var id = string.Empty;
        if (storageModel != null)
        {
            id= storageModel.DocumentId;
        }
        var document =await documentClient.DownloadDocumentAsync(id).ConfigureAwait(false);
        var contentType = "application/pdf"; // Set the content type of the file
        var fileName = "Copy_InsuranceClaimForm.pdf"; // Set the file name
        Response.Headers.Add("Content-Disposition", "Attachment; filename=" + fileName);
        return File(document, contentType, fileName);
    }

    [HttpPost]
    // Create EmbedSignLink for the document
    public async Task<IActionResult> SignDocument(TemplateDetails templateDetails)
    {
        // Collect all the basic form fields from the model
        var storageModel = HttpContext.Items["TemplateDetails"] as TemplateDetails;
        if (storageModel != null)
        {
            templateDetails.PolicyType = storageModel.PolicyType;
            templateDetails.FullName = storageModel.FullName;
            templateDetails.Gender = storageModel.Gender;
            templateDetails.DOB = storageModel.DOB;
            templateDetails.PolicyNumber = storageModel.PolicyNumber;
            templateDetails.DamageDetails = storageModel.DamageDetails;
            templateDetails.InsuredObject = storageModel.InsuredObject;
        }
        templateDetails.TemplateId = this.templateId;
        // templateDetails.ClaimDate = DateTime.Now.ToString("MM/dd/yyyy");
        var documentDetails = new SendForSignFromTemplate()
        {
            Title = "Cubeflakes - Insurance Claim Form",
            TemplateId = templateDetails.TemplateId,
            DisableEmails = true,
            Roles = new List<Roles>()
            {
                new Roles
                {
                    SignerName = templateDetails.FullName,
                    SignerEmail = templateDetails.EmailAddress,
                    RoleIndex = 1,
                    SignerType = SignerType.Signer,
                    ExistingFormFields = new List<ExistingFormField>()
                    {
                        new ExistingFormField()
                        {
                            Id = "txtClaimType",
                            Value = templateDetails.PolicyType,
                        },
                        new ExistingFormField()
                        {
                            Id = "txtPolicyNumber",
                            Value = templateDetails.PolicyNumber,
                        },
                        new ExistingFormField()
                        {
                            Id = "txtFullName",
                            Value = templateDetails.FullName,
                        },
                        new ExistingFormField()
                        {
                            Id = "rbnMale",
                            Value = templateDetails.Gender,
                        },
                        new ExistingFormField()
                        {
                            Id = "txtDateOfBirth",
                            Value = templateDetails.DOB.ToString("MM/dd/yyyy"),
                        },
                        new ExistingFormField()
                        {
                            Id = "txtPermanentAddress",
                            Value = templateDetails.Address,
                        },
                        new ExistingFormField()
                        {
                            Id = "txtZipCode",
                            Value = templateDetails.ZipCode,
                        },
                        new ExistingFormField()
                        {
                            Id = "txtEmail",
                            Value = templateDetails.EmailAddress,
                        },
                        new ExistingFormField()
                        {
                            Id = "txtPolicyNumber",
                            Value = templateDetails.PolicyNumber,
                        },
                        new ExistingFormField()
                        {
                            Id = "txtPhoneNumber",
                            Value = templateDetails.PhoneNumber,
                        },
                        new ExistingFormField()
                        {
                            Id = "attachLicense",
                            Value = templateDetails.DrivingLicense,
                        },
                        new ExistingFormField()
                        {
                            Id = "txtInsuredObject",
                            Value = templateDetails.InsuredObject,
                        },
                        new ExistingFormField()
                        {
                            Id = "txtDamageDetails",
                            Value = templateDetails.DamageDetails,
                        },
                        new ExistingFormField()
                        {
                            Id = "txtInsuredName",
                            Value = templateDetails.FullName,
                        }
                    }
                }
            }
        };
        // Create document from Template with the new form fields
        DocumentCreated documentCreated = null;
        try
        {
            documentCreated = await this.templateClient
                .SendUsingTemplateAsync(sendForSignFromTemplate: documentDetails).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        templateDetails.DocumentId = documentCreated.DocumentId; // created in the previous step
        //Create embedded Sign URL from the document created
        EmbeddedSigningLink embeddedSignUrl = this.documentClient.GetEmbeddedSignLink(
            documentId: templateDetails.DocumentId,
            signerEmail: templateDetails.EmailAddress,
            DateTime.Now.AddDays(30),
            redirectUrl:$"https://{this.Request.Host}/Home/Responses");
        templateDetails.SignLink = embeddedSignUrl.SignLink; // This SignLink will be loaded into the iframe
        if (storageModel != null)
        {
            storageModel.SignLink = templateDetails.SignLink;
            storageModel.DocumentId = templateDetails.DocumentId;
        }
        return View(templateDetails);
    }
}