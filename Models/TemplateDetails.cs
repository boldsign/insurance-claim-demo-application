using System.Text.Json.Serialization;

namespace InsuranceDemo.Models;
    public class TemplateDetails
    {
        [JsonPropertyName("policyType")] public string PolicyType { get; set; }
        [JsonPropertyName("policyNumber")] public string PolicyNumber { get; set; }
        [JsonPropertyName("fullName")] public string FullName { get; set; }
        [JsonPropertyName("gender")] public string Gender { get; set; }
        [JsonPropertyName("dateOfBirth")] public DateTime DOB { get; set; }
        [JsonPropertyName("ClaimDate")] public string ClaimDate { get; set; }
        [JsonPropertyName("address")] public string Address { get; set; }
        [JsonPropertyName("zipCode")] public string ZipCode { get; set; }
        [JsonPropertyName("emailAddress")] public string EmailAddress { get; set; }
        [JsonPropertyName("phoneNumber")] public string PhoneNumber { get; set; }
        [JsonPropertyName("insuredObject")] public string InsuredObject { get; set; }
        
        [JsonPropertyName("drivingLicense")] public string DrivingLicense { get; set; }
        [JsonPropertyName("damageDetails")] public string DamageDetails { get; set; }
        [JsonPropertyName("signLink")] public string SignLink { get; set; }
        [JsonPropertyName("documentId")] public string DocumentId { get; set; }
        [JsonPropertyName("templateId")] public string TemplateId { get; set; }
    }