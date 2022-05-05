using EnvDTE;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ISoft.Coder
{
    public partial class SqlServerClassGenWrapper
    {
        /// <summary>
        /// select '{ "' + AirtableId + '", "' + name + '" },' from ManagementTrigger
        /// </summary>
        private static Dictionary<string, string> _Triggers = new Dictionary<string, string>()
        {
{ "reckkLu4zY3ZQq18G", "Archive Listings" },
{ "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing " },
{ "recAwschExegCxyPv", "Bond refund - Silent bond claim" },
{ "recrXtsG7zLhEEH1Y", "Bond Refunds" },
{ "recz07gQwvXNAe1ut", "Change Of Occupants" },
{ "recMxPUoy48RNdMJY", "Change of Ownership - Financials" },
{ "recdDAFpR7P9BZT63", "Change of Ownership - Front of House" },
{ "recVdNbgKQs5kQysD", "Change of ownership - Property Manager" },
{ "rec3kMWgFCTdQDVNr", "Final inspection" },
{ "recUGZNj1CBvk6xC0", "Find a Tenant - New Management" },
{ "recveQfCQNrmxLTLw", "Find a Tenant - Relet" },
{ "recQDUyuzQCTfEqUb", "General addendums - management and tenancy agreement" },
{ "rec552xxtTYhL0OUL", "Insurance claim - Rent loss" },
{ "recRYAdVQ7oKFeXRZ", "Letting only - Front of House" },
{ "rec7Ts947EFHbSdTT", "Letting only - Leasing " },
{ "reci2VK2nPUi5WNG7", "Management Financials Settled" },
{ "recS7t5XgcjtP6czs", "Management lost" },
{ "reczUiVevW9QXrFty", "Management Lost - Landlord Offboard" },
{ "recRhqQ8cFKmwrc1E", "Management Lost - ToopFix update " },
{ "recODcelPqqjKNlWb", "Management Takeover" },
{ "recqL05XEPWRAyN7N", "Management Transfer" },
{ "rec58orUTPPCa5hkG", "New management - Front of House" },
{ "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator" },
{ "reckrmSdfKx35zqYo", "New management - Property Manager" },
{ "recBOXKJRV9wtZd5k", "New tenant - Ingoing Inspection" },
{ "recw9eRfgHy6OOOgP", "New Tenant - Water invoicing" },
{ "recnPTSIEucW8pZV6", "Reinstated management" },
{ "rec3puMpgXTmmRq2L", "Rent Arrears - 16 days" },
{ "recjjTN3bn9GkpPJn", "SACAT Hearings" },
{ "recuONcwG7nSwXNYk", "Tenant confirmed - New Management" },
{ "recTUadxm4JaCRJxy", "Tenant confirmed - Relet" },
{ "recVNIeKSYALwEWI2", "Tenant In Home" },
{ "recK695XnaMlEIN7D", "Tenant starts - New Management" },
{ "recHgZI6oKbaclVf7", "Tenant starts - Relet" },
{ "recoxn64Do6Toykyg", "Under Renovation" },
{ "recZTFJlaZYGrRYwd", "Vacating Tenant" },
        };

        /// <summary>
        /// select 
        /// '( "' +
        /// m.AirtableId + '", "' +
        /// m.Name + '", "' +
        /// t.AirtableId + '", ' +
        /// CONVERT(varchar, isnull(t.Ordinal, 0)) + ', "' +
        /// t.Name + '", "' +
        /// isnull(nm.AirtableId, '') + '"' +
        /// ' ),'
        /// from ManagementTaskTemplate t
        /// join ManagementTrigger m on t.TriggerId = m.Id
        /// left join ManagementTrigger nm on t.NextTriggerId = nm.Id
        /// order by m.name, m.AirtableId, t.Ordinal
        /// </summary>
        private static List<(
            string TriggerId,
            string TriggerName,
            string Id,
            int Ordinal,
            string Name,
            string NextTriggerId)> _Tasks => new List<(string TriggerId, string TriggerName, string Id, int Ordinal, string Name, string NextTriggerId)>()
            {
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recQt23PdWyoM9fwk", 1, "SACAT application required - landlord advised", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recxHLBqllkyKpIMe", 2, "Application prepared & submitted", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recPwcxunSkNukB17", 2, "SACAT invoice received - set task for Finance Manager", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "reclifelMIRp5JFIA", 4, "Notice of hearing received and diarised", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recVfKEDQBqbZ2G1D", 5, "Landlord notified and invited to attend the hearing", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recF0qgURnbXSQFhP", 6, "Prepare checklist & documents for hearing", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recdPxNPzNonioDLJ", 7, "Hearing attended and attendance charged", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recSzJ9BnQmo5tvVe", 8, "Is the determination for the amounts claimed", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recUI6RfHt5QZUuH3", 9, "Call the landlord and advise of outcome", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recLVcDzVMhdFkBDv", 10, "SACAT / Magistrates order received", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recOavUumb5p0OMnv", 11, "Copy sent to the landlord", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recu4WyYYGmpws9hN", 12, "Copy of order sent to Corporate Support", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recG9Y5quO23qydgk", 13, "Bond refund received", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recnXbJmJ3ddsqMvW", 14, "Landlord advised", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recgdMMQrgq6khSB0", 15, "Tenant archived in PropertyMe", "" ),
( "recnOgfZkcnlF2mDk", "Bond refund - SACAT hearing ", "recRzvIiNLcdtGfLw", 16, "Download checklsit & save to PropertyMe", "" ),
( "recAwschExegCxyPv", "Bond refund - Silent bond claim", "rec7Sdj6n4zAiFbjn", 1, "Silent bond claim/Notice of claim submitted", "" ),
( "recAwschExegCxyPv", "Bond refund - Silent bond claim", "recFmQJuaNYkKMx1q", 2, "Landlord updated with status of bond", "" ),
( "recAwschExegCxyPv", "Bond refund - Silent bond claim", "rec94u7v4eaWNbEZt", 3, " Claim denied - SACAT hearing required", "" ),
( "recAwschExegCxyPv", "Bond refund - Silent bond claim", "recehuQqq6WZmxESU", 4, "Bond refund received", "" ),
( "recAwschExegCxyPv", "Bond refund - Silent bond claim", "recSN5qvqk0WCgooC", 5, "Landlord advised refund has been received", "" ),
( "recAwschExegCxyPv", "Bond refund - Silent bond claim", "recELAl0AbXMhYI1m", 6, "Insurance claim for rent loss required", "" ),
( "recAwschExegCxyPv", "Bond refund - Silent bond claim", "rectsZMePmVXOeILi", 7, "Download checklist & save to PropertyMe", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recNqFuGyeFUPcWK9", 1, "Invoices have been received for work completed", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recDnWi5wSa5ltIul", 2, "Has the tenant paid rent past the vacate date?", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recjEY46kxi18ZP5g", 3, "Overpaid rent - set task for Finance Manager to arrange refund", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recD0f2ko6bf89d2z", 4, "Does the tenant owe any rent?", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recTQjXx56vPDSuLF", 5, "Is there any claim for rent differential? (account code 101)", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recOCidMhkQY0w5u6", 6, "Is there any claim for water usage &/or supply? (account code 166)", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recXw1QvhAlelAUPi", 7, "Is there any claims for repairs?  (account code 180)", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recWiMtLrTOaDpv9j", 8, "Is there any claims for cleaning?  (account code 162)", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recQXi6tpU0C1Okln", 9, "Is there any claims for gardening? (account code 175)", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "rec2GOOxUUDjQOmBH", 10, "Is there any claims for key cutting or changing locks? (account code 178)", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recXs93upxk3AZArs", 11, "Is there any claim for compensation? (account code 172)", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "reclsXYAOFIQpBLXm", 12, "Is there any claims for lease break costs - letting fee (account code 173)", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recqr59TdY15WyS62", 13, "Is there any claims for advertising costs?  (account code 174)", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recV28WIT1FrIX3Aj", 14, "Does the tenant have any funds under 'Deposits' in Property Me to offset claim", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recccL6dXi7KSHULy", 15, "Link to bond refund summary sheet", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recCDkjWVnTU3sZcp", 16, "Claim details emailed to tenant", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recby4b6khUmhh89w", 17, "Claim details emailed to landlord", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recUJeyNfyi8X0wL4", 18, "Internal invoices for bond claims raised", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recTl0FW6d3hPCDar", 19, "Is there a deposit to be refunded to tenant - contacted for their bank account details", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recAIzNEFkLjyfTME", 20, "Deposit has been disbursed to tenant", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "reciO4vjF6qaWNEiq", 21, "Bond refund requested / authorised on Bonds On-Line", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recTLUmWCz5QkqPFi", 22, "Bond refund received", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "rec1MqPBVac4UkzHl", 23, "Landlord advised that bond monies have been received & receipted", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "rec70Iuh6QPqmW1IS", 24, "Do you need to lodge a silent bond claim?", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recubV33AWqWC4VS6", 25, "SACAT hearing required - tenant has disputed", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recZbAl4kvuRMg0e4", 26, "Check if an insurance claim for loss of rent is required", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recvM4wHvKMlj6Asn", 27, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recrXtsG7zLhEEH1Y", "Bond Refunds", "recOzqPWdli8ba2DD", 28, "Tenant archived in PropertyMe", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "recohbNC0egy2AnkN", 1, "Tenant request received ", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "rec575xWhsfDWcjP2", 2, "Tenant has provided a completed 'Change of Tenant' form", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "recFBRRzPDerc8O3b", 3, "Application received from new occupant", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "recDuTYGRs4TwY8HG", 4, "Does the vacating tenant have a Housing SA bond guarantee", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "recEoVGX1eLwX2AEo", 5, "Do you need to set a task for the Finance Manager about the bond", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "recal1H2WAQ3dgAQE", 6, "Has the bond been paid for the amount equivalent to the Housing SA bond", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "reckJYUFP3AodjCEo", 7, "Refund the bond to Housing SA for the vacating tenant", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "recQXtst0nt4vIqeB", 8, "Application processed and approved by landlord", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "recNegQD70RQOe4rT", 9, "Draw up an addendum and send to tenants for signing", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "rec8AHjChQhxnJqM3", 10, "Addendum signed by tenants", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "rechaTwuh1T9X71xY", 11, "Signed on behalf of the landlord", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "rec6BMwNTw1bnFr5H", 12, "Copy saved to PropertyMe ", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "rec2fhvSKrlYb6yQi", 13, "Email copy of the addendum to the tenants", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "reczAOattVcGzuhSS", 14, "Change of tenant form saved to Property Me & posted to CBS", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "rec1VfoHtMoMDiU3j", 15, "Property Me updated with tenant details.", "" ),
( "recz07gQwvXNAe1ut", "Change Of Occupants", "rec992zwgmLJehJQk", 16, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "recnud9S8VDrr9jyW", 1, "Place funds on hold so rent is not paid out to owner past settlement date", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "recoQH2Ik8GjWH8e4", 2, "Are we retaining management?", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "rec5hdg2HNsZYftCL", 3, "Confirmation email from Conveyancer received", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "recItdToX9r4nYiFB", 4, "Process Change of ownership in PropertyMe", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "rec3FsPdlTXMmVlSg", 5, "Manually calculate management fees on rental adjustment", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "recuhELFkbxpRTvft", 6, "Enter lost or gained comments in Property Me", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "recEgzk7rBzfxDgLn", 7, "Pay out funds held to vendor", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "recojrr1dtQN2CFmW", 8, "Are we losing management?", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "recAxUo4bSNvs5PUC", 9, "De-link DEFT reference so no further funds are received", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "recEoPLYIIbL2zqNK", 10, "Pay out funds held to vendor", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "recXQlaTXUOP15qFy", 11, "Advise FOH process has been completed by reassigning task", "" ),
( "recMxPUoy48RNdMJY", "Change of Ownership - Financials", "recVnn4yhQ4ihgnDO", 12, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recdDAFpR7P9BZT63", "Change of Ownership - Front of House", "recmVt400Ka9zIqyU", 1, "Enter owner contact details into Property Me", "" ),
( "recdDAFpR7P9BZT63", "Change of Ownership - Front of House", "reccuaQ9vBEiXOmLQ", 2, "Set a task for Finance Manager advising property has been sold & expected settlement date", "" ),
( "recdDAFpR7P9BZT63", "Change of Ownership - Front of House", "recE7auRqNUSsOjBT", 3, "Place an alert against the property as to expected settlement date", "" ),
( "recdDAFpR7P9BZT63", "Change of Ownership - Front of House", "recCB1q80PnEoLppF", 4, "Finance Manager has confirmed settlement has taken place", "" ),
( "recdDAFpR7P9BZT63", "Change of Ownership - Front of House", "recu7KOQ7x3YZIXv6", 5, "Complete entering management agreement updates", "" ),
( "recdDAFpR7P9BZT63", "Change of Ownership - Front of House", "rec1IUuaAbd8wRgre", 6, "Enter owner details into REX", "" ),
( "recdDAFpR7P9BZT63", "Change of Ownership - Front of House", "recggL4gaE9Yj7zg7", 7, "Save management agreement to PropertyMe", "" ),
( "recdDAFpR7P9BZT63", "Change of Ownership - Front of House", "reca7lyGLVlAITdc9", 8, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recVdNbgKQs5kQysD", "Change of ownership - Property Manager", "recKc1FAic1FxDDA1", 1, "Email 'new management' thankyou letter to owner", "" ),
( "recVdNbgKQs5kQysD", "Change of ownership - Property Manager", "rec1F8A9uAvF0a6e4", 2, "Email tenant to advise of change of ownership", "" ),
( "recVdNbgKQs5kQysD", "Change of ownership - Property Manager", "recXTop2wTx4ctoxd", 3, "SA Water - is a change of postal address required?", "" ),
( "recVdNbgKQs5kQysD", "Change of ownership - Property Manager", "recYCq1kQiqp5odJb", 4, "Council rates - is a change of postal address required", "" ),
( "recVdNbgKQs5kQysD", "Change of ownership - Property Manager", "recfRb5RVDTpm0uR7", 5, "Strata - is a change of postal address required", "" ),
( "recVdNbgKQs5kQysD", "Change of ownership - Property Manager", "recbBSR7ktYAQ3Hgc", 6, "Routine inspection fee - does it need to be recalculated", "" ),
( "recVdNbgKQs5kQysD", "Change of ownership - Property Manager", "rect42I1jeBR3Zuoa", 7, "Check if owner wants a Compliance & Preventative Maintenance Package", "" ),
( "recVdNbgKQs5kQysD", "Change of ownership - Property Manager", "recYwwyUJmejEdyqm", 8, "Update the maintenance instructions in ToopFix", "" ),
( "recVdNbgKQs5kQysD", "Change of ownership - Property Manager", "rec11lZkXzOx42Gha", 9, "Update bond online with new owner details", "" ),
( "recVdNbgKQs5kQysD", "Change of ownership - Property Manager", "recXgAwnbGdcRSAaN", 10, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "recoRCIpocTQUjHyE", 1, "Final inspection completed & reviewed", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "recynMA7yfNr5IaOL", 2, "Reviewed against the ingoing inspection report", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "recz6bbFMJSs7D53F", 5, "Are there any cleaning/repairs required (list under personal notes)", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "recEtiP1DYiGdVWep", 6, "Email tenants if no issues found", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "rec5K6Atsr8NTYfmR", 7, "Report emailed to the tenant due to issues found", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "recGMMACGEy2NlQ3r", 8, "Report emailed to the landlord", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "recI4Pj7hFKP368BS", 9, "Rent owing - add label 'arrears bond claim' to property", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "recyDACoUdbJ96oC1", 10, "Work orders issued for cleaning/repairs/gardening required", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "reckRoKkn7Ng4UyTB", 11, "Has the tenant returned all keys, remotes etc", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "rec4LFF3mvDWjnOFK", 12, "Tenant keys checked,  tagged and placed in tenant key box", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "recPnlmXfbjPRNp6t", 13, "Are there any abandoned goods?", "" ),
( "rec3kMWgFCTdQDVNr", "Final inspection", "recqpK921JxXjuVFt", 14, "Download checklist & save to PropertyMe before deleting", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recm1adXGMyvKDu8H", 1, "Weekly REA summary sent to landlord", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "reckLGKw3rrd3AEvl", 2, "Applications received", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "rec0Pxig9Mwaq2CWN", 3, "Applications reviewed", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recQNjjMY9NfkLzrN", 4, "Complete reference script & application cover sheet", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recHl880iuMgccCDq", 5, "Application referred to landlord", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recaX9O5On9EfDTQ8", 6, "Application accepted by landlord", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recWReIma9Pl1jymR", 7, "Call to successsful tenant", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "reca7nZOw41Hg6AsI", 8, "Call to unsuccessful tenant", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recEaQOV9xGE2OH0g", 9, "Property availability date confirmed", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recyw4Z8VeGrKKv2V", 10, "Send confirmation email to landlord", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "rec7komkrgG7jMKQc", 11, "Confirm Special conditions required on tenancy agreemnet", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recSWlTu5brkMLJFf", 12, "DEFT reference number - record on spreadsheet", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recS3s4Kx1NSvLmyU", 13, "Enter tenant details onto PropertyMe", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recwE7wFK175BuOzN", 14, "Tenancy agreement sent to tenant with confirmation email", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recWr6hAxummoA70f", 15, "Bond and rent received in trust", "" ),
( "recUGZNj1CBvk6xC0", "Find a Tenant - New Management", "recAkJOhFGU5tV7cV", 16, "Download checklist & save to Property Me", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recRDSweEsC2aloqo", 1, "Is this a lease break ?", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "rec0qzzLXoHnOvQPd", 2, "Relet - contact owner to discuss marketing", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recPndOrwv3pL9wvy", 3, "Special conditions confirmed with landlord", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recyP4q7v4pL2IIkg", 4, "Confirmation email sent to landlord with relet instructions", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recpxnjUC4yW3Q3le", 5, "Property launched on all applicable websites", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recnWJrOlhizAeV6N", 6, "Send link of toop.com.au Ad to landlord in case they wish to change ", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recoQSZbYjV61G6VN", 9, "Application received x1", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "reciiefbUd7caS20N", 10, "Send Weekly REA summary to Landlord", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recN0mrB1QGUwWeIr", 11, "Application   review", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recQokh9zzQtSVfcg", 12, "Complete attached reference script & application cover sheet", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recHXvITWLFTFl60q", 13, "Applications referred to landlord", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recdm48o36tasNqkk", 14, "Application accepted by landlord", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recwfBAvUbbWpinUi", 15, "Call to successful tenant", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recNFZ6El9A1fO0lX", 16, "Advise unsuccessful tenant/s", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recUnPatAhUWUsFfz", 17, "Property availability date confirmed with landlord or property manager", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "rec86BNyVkwvh9txZ", 18, "Confirmation email to landlord with tenancy details", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recAELWvaaI4I7cA0", 19, "Confirm all special conditions required on the tenancy agreement", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recPMOylKx8gatElg", 20, "DEFT reference number - record on spreadsheet", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recr9SHXRxPEPQjyK", 21, "Enter tenant details into PropertyME", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "rec1Kc95hVF2yUycM", 22, "Tenancy agreement sent to tenant with confirmation email", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recCEU8A97nNMhzyz", 23, "Bond and Rent Received in Trust", "" ),
( "recveQfCQNrmxLTLw", "Find a Tenant - Relet", "recVAOKHrs1nuincK", 24, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recQDUyuzQCTfEqUb", "General addendums - management and tenancy agreement", "recflifHU7eCfWFKo", 1, "Tenant or owner request received", "" ),
( "recQDUyuzQCTfEqUb", "General addendums - management and tenancy agreement", "recP06KbkyaoK939J", 2, "Tenant request - referred to landlord", "" ),
( "recQDUyuzQCTfEqUb", "General addendums - management and tenancy agreement", "recDSpr1oGBsFf0w0", 3, "Owner request - referred to General Manager and approved", "" ),
( "recQDUyuzQCTfEqUb", "General addendums - management and tenancy agreement", "recNJdV4cS6wqwbZZ", 4, "Draw up an addendum and sent to tenant/landlord", "" ),
( "recQDUyuzQCTfEqUb", "General addendums - management and tenancy agreement", "rectAesWpDT5IFRJs", 5, "Addendum signed and returned by landlord/tenant", "" ),
( "recQDUyuzQCTfEqUb", "General addendums - management and tenancy agreement", "rec4TcqxrEjn3jZp9", 6, "Addendum signed by Toop team member", "" ),
( "recQDUyuzQCTfEqUb", "General addendums - management and tenancy agreement", "recDxyKjIfsAjGvoO", 7, "Copy saved to PropertyMe", "" ),
( "recQDUyuzQCTfEqUb", "General addendums - management and tenancy agreement", "recy96gBJ0ZvdGEz2", 8, "Fully signed copy emailed to client", "" ),
( "recQDUyuzQCTfEqUb", "General addendums - management and tenancy agreement", "recYhJM7gfPcdEq1k", 9, "PropertyMe updated with change of conditions", "" ),
( "recQDUyuzQCTfEqUb", "General addendums - management and tenancy agreement", "recQBu7OlM1oGLTmf", 10, "Download checklist & save to PropertyMe", "" ),
( "rec552xxtTYhL0OUL", "Insurance claim - Rent loss", "recJh7uAMe3CI5Rm3", 1, "Is a claim for rent loss required?", "" ),
( "rec552xxtTYhL0OUL", "Insurance claim - Rent loss", "recsMa6yGQqcjCCFM", 2, "Toop PM to lodge claim", "" ),
( "rec552xxtTYhL0OUL", "Insurance claim - Rent loss", "recsjFTZVarDIgCSj", 3, "Landlord to lodge their own claim - documents sent", "" ),
( "rec552xxtTYhL0OUL", "Insurance claim - Rent loss", "recxnO0EYgnIhmuTw", 4, "Claim submitted by Toop Property Management", "" ),
( "rec552xxtTYhL0OUL", "Insurance claim - Rent loss", "recXgdpqDd2tTQBQZ", 5, "Landlord advised claim submitted", "" ),
( "rec552xxtTYhL0OUL", "Insurance claim - Rent loss", "recNBA97qM0eqJNC8", 6, "Funds received and receipted", "" ),
( "rec552xxtTYhL0OUL", "Insurance claim - Rent loss", "recNfff6qXNBtNOgH", 7, "Landlord advised claim finalised &  funds received", "" ),
( "rec552xxtTYhL0OUL", "Insurance claim - Rent loss", "recgi9mbNklicZSM4", 8, "Tenant archived", "" ),
( "rec552xxtTYhL0OUL", "Insurance claim - Rent loss", "rec7gjPCOqN54d1vm", 9, "Download checklist & save to PropertyMe", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "recACM2g9XFMn5eLh", 1, "Enter property details into PropertyMe", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "recEtegyYbBPrg1ZU", 2, "Enter owner contact details", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "reconHyPHEQFchjeH", 3, "Add the referral source (gained management)", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "recfabGIu3jzUunTz", 4, "Tag the owner with Letting Only", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "recrNDQ85y4RWL2xU", 5, "If a Toop Manual referred, add label", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "recTM9NjCTtE1B8tA", 6, "Special conditions apply?  Add label to the property", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "recXawkiAsiw6eV0f", 7, "Enter special conditions in the property notes", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "rect7AkZ71nSWSXc7", 8, "Add a label to verify the landlord's policy on pets", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "recVi7DPCaAsteyDL", 9, "Attach a photograph of the property into PropertyMe", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "rec3ExGKRANTjkqxV", 10, "Thankyou for you business email to be sent", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "rec8o8R2OBYLff0r0", 11, "Save Management Agreement into PropertyMe", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "rec19Y9PexMT3z5zY", 12, "Enter owner details into REX", "" ),
( "recRYAdVQ7oKFeXRZ", "Letting only - Front of House", "recfSSoh5xCdegHLd", 13, "Download checklist & save to PropertyMe", "" ),
( "rec7Ts947EFHbSdTT", "Letting only - Leasing ", "recLXb9s8fIkQR6L9", 1, "Prepare a change of ownership form", "" ),
( "rec7Ts947EFHbSdTT", "Letting only - Leasing ", "recR6A9GUOXRkpZRy", 2, "Set a task for the Finance Manager to issue invoice", "" ),
( "rec7Ts947EFHbSdTT", "Letting only - Leasing ", "recYQMH3ZaszReZiP", 3, "Prepare documents to be sent to the landlord", "" ),
( "rec7Ts947EFHbSdTT", "Letting only - Leasing ", "recbYfr6QAWcd00Qx", 4, "Letting only - email documents to landlord", "" ),
( "rec7Ts947EFHbSdTT", "Letting only - Leasing ", "recHeJhKh8mjc16yR", 5, "Download checklist & save to PropertyMe", "" ),
( "reci2VK2nPUi5WNG7", "Management Financials Settled", "recyI6ufjIOKaW7s5", 1, "Check there are no outstanding disbursements against the owner and balance is zero", "" ),
( "reci2VK2nPUi5WNG7", "Management Financials Settled", "recEHRXP8Yh3O8LlG", 2, "Send financial year statement to landlord", "" ),
( "reci2VK2nPUi5WNG7", "Management Financials Settled", "recn01wcsEJQVaend", 3, "Archive property In PropertyMe", "" ),
( "reci2VK2nPUi5WNG7", "Management Financials Settled", "recaTcpiax2JnYdZA", 4, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recfYfEEodke3rmK5", 1, "Confirm written notification from the landlord", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recNtVOzZDGjVf7p2", 2, "Remove property from Routine Inspection Spreadsheet", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recEwm39voQwcEBNB", 3, "Remove routine inspection date from PropertyMe", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recGgcqQ6lOV71jIk", 4, "Enter lost management date into PropertyMe (not the reason)", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recHIbw4l6vybsnK4", 5, "Send email to General Manager to advise details", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "reccernEz8Fbdqsj8", 6, "Is this a management transfer to another agent or landlord?", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recOdcA5CVHpwnbe9", 7, "If management transfer, has landlord given correct notice", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recXPrMXujFE2igma", 8, "If management transfer, are termination fees applicable", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "rec6Nj5gPVRUnrSNX", 9, "If fees to be charged, set a task for the Finance Manager", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recdw6sU923bKbbHc", 10, "Send confirmation email to landlord", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recfPd0VdECmBLlQY", 11, "ToopFix outstanding work orders to be checked", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "reckzqLzfuCjlf51I", 12, "Put funds on hold for outstanding maintenance", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recAgcHLtTC950B7B", 13, "Attached 'Not for Relet' label to property", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recqHn6YJsKQsq2Uc", 14, "Create an orange alert to show on bills feature", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recHwbDDQUlcUtVml", 15, "Management officially lost", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recEQn4tUn7Yql2IW", 16, "Has this property been sold & Toops retaining management", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "recmaNrF3ZCPklwc4", 17, "Has property been sold to an investor?", "" ),
( "recS7t5XgcjtP6czs", "Management lost", "rec6tRuMXqRPYuebb", 18, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "reczUiVevW9QXrFty", "Management Lost - Landlord Offboard", "receTTL4OSTupKtyQ", 1, "Delete property from routine spreadsheet", "" ),
( "reczUiVevW9QXrFty", "Management Lost - Landlord Offboard", "recrG5tEyaXKktR6f", 2, "Remove admin fee against landlord", "" ),
( "reczUiVevW9QXrFty", "Management Lost - Landlord Offboard", "recHcxMKv8AkW8ZL7", 4, "Remove key from key cabinet, remove tag & delete key number in PropertyMe", "" ),
( "reczUiVevW9QXrFty", "Management Lost - Landlord Offboard", "recYZ6b6P83Fx8wXz", 5, "Photocopy all keys, cards, remotes", "" ),
( "reczUiVevW9QXrFty", "Management Lost - Landlord Offboard", "recKojTe7olXEkJ64", 6, "Print attached key receipt for owner to sign", "" ),
( "reczUiVevW9QXrFty", "Management Lost - Landlord Offboard", "recqZ02oXx4HB8qq1", 7, "Keys & key receipt placed at FOH for collection", "" ),
( "reczUiVevW9QXrFty", "Management Lost - Landlord Offboard", "recClTQIFLvfOkCYp", 8, "Email sent to  landlord re change of postal adddress", "" ),
( "reczUiVevW9QXrFty", "Management Lost - Landlord Offboard", "recwUzaNyTYBxGVb4", 15, "Inactivate Property in Inspection Manager", "" ),
( "reczUiVevW9QXrFty", "Management Lost - Landlord Offboard", "reclZSth7Etug2gO7", 16, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recDNcyfGxkXSJeSW", 1, "Diarise to collect tenancy documents and keys from previous agent/owner", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recWqWJbhoXELQtu7", 2, "Check all documents and keys received from previous agent/owenr", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recr6lnS7TJigKKpQ", 3, "Call the tenant to advise of management takeover", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recHTADJYDRIwtc6C", 4, "Confirm with landlord all documents collected from previous agent", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "rectCGDIoa3xnbyjQ", 5, "Enter tenant details into PropertyMe", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recrpvV7zvSgHXBfh", 6, "Enter activity note re previous agent details", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recSaTUdw4UL4iqWD", 7, "Allocate a DEFT reference number", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "rec9oMgyYAWitHI23", 8, "Create a receipt warning re letting fee", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recNM8O0vK5tm90Md", 9, "Enter Administration Fee against the owner", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "rec2N5eGKsj7cWxsi", 10, "Enter routine inspection date in PropertyMe", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recdHC9Y2HOndpO7Z", 12, "Send confirmation email to tenant", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "reccJD2jqq5Gt3Lnx", 13, "Allocate key number, tag keys and place in key cabinet", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "rec7OqscN2qoIHqkz", 14, "Schedule routine inspection to be conducted within the first month ", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recXJFbb0SqJR7lrK", 15, "Calculate routine inspection fee & enter into PropertyMe", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "rec8X8JvK5LYPjYwM", 16, "SA Water - change of postal address required?", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recmVhJXhPchP9Qsc", 17, "Council - change of postal address required", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "rectz8hGHI5FixaxH", 18, "Strata - change of postal address required", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recn2JKV3NdbBoGrZ", 19, "Landlord Insurance - change of postal address required", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recxmWBwjTkqmzM5T", 20, "Import bond request on Residential Bonds On-line", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recwpnPdj9YjoDSIp", 21, "Enter property onto RENEW ", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "receuHNjNTQ4gmhJU", 22, "Check Bonds On-Line to ensure bond change has been authorised", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recv97QiCg57yO0Br", 23, "Scan all documents and save in PropertyMe", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "recek2hyktjgOlwv2", 24, "Check in with the tenant in 3 days to ensure they received their email", "" ),
( "recODcelPqqjKNlWb", "Management Takeover", "rec81H0uk9FaNOCYM", 25, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "recxOuKFIgNLzjqt3", 1, "Contact owner/new agent to make an appointment to collect docs & keys", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "recEwyjTn2sPByocw", 2, "Check last routine inspection date & conduct another if required", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "recUIGu7WiPCD4QMe", 3, "Email sent to tenant advising of change of agent", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "recKBGMn2x2UeWdOG", 4, "Print a copy of tenancy agreement & lease renewal documents ", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "reciEinzFimviaAb0", 5, "Prepare 'Change of Ownership' form in REA forms", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "rece4b27TC7XDpo1M", 6, "Print a copy of the full rent history", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "recJqHOEXKH6M8zFb", 7, "Print a copy of any outstanding invoices payable by tenant", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "recsKPy2AgrxMwaQv", 8, "Print a copy of the key receipt signed by the tenant", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "recmRQ5BN8GiPGBCJ", 9, "Remove keys from key cabinet, untag & label with property address", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "recE9vJuRKuuTSUyW", 10, "Print a copy of any outstanding rates & taxes invoices payable by landlord", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "rec7NqapuTwbZhbpf", 11, "Tenant contact details for new agent / landlord", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "recYD0x9Ps4rhU9XC", 12, "Handover completed - enter vacating date against tenant in PropertyMe", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "recT62VKIVefNU6bV", 13, "Send confirmation email to landlord", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "rechCkSdud7E5hXwQ", 14, "Archive tenant in PropertyMe", "" ),
( "recqL05XEPWRAyN7N", "Management Transfer", "recde6jJKW6j7r6R0", 15, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recdRHm15zfskUAay", 1, "Assign to Property Manager", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recwbPoyNhI2uwIeR", 2, "Is this a letting only?", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recB6le7D24oZHoYy", 3, "Is this a management takeover?", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recFejiaVRLFp4ijT", 4, "Is this a change of ownership - see notes", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recWnCVoEXhj2F6q4", 5, "Enter property address into PropertyMe", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "rectyWO1PdZ7fmipx", 7, "Enter property details (refer to page 11 of agreement))", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recyyeRAg3E9OiI6d", 8, "Enter  special conditions & maintenance authority", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recw7Aaf3hLgkngMw", 10, "Add label: Special conditions apply?  Or appliance manuals", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recl9sbrFlGfPmmWK", 11, "Add label to verify the landlord's policy on pets", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recAeO4T9o4JTKTAF", 12, "Is the property furnished? Add label & inventory fee", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "rec3RDZS4WbYUxXpv", 12, "Add label to property for water charges payable by tenant", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "reckj719Chnrhuzyl", 14, "Add label for Sales Referral", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recsgYgW2YdQVN7Ho", 15, "Attach photograph of the  property into PropertyMe", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recbU5JsG69CiNO5e", 16, "Enter owner contact details", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recWQMR0XKqhhO1av", 17, "Tag the owner contact with the Team Name & 'multiple LL' if applicable", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recef4ucNvANZTkw7", 18, "Assign owner to property", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recoeeCIVUvrvPc3j", 19, "Enter details in folio tab", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "rec3r9pfkPEupi5FP", 20, "Add the referral source, gained management date and who listed the property & add a label to the property", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recwUhnNViPThZzoz", 21, "Enter owner fees", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recUGqFj9TnGxEvgD", 22, "Enter owner's bank account details", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "rec2XwpCiCIq79QyR", 23, "Create an alert against the owner with the strata details", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "reciNbI3bSQHPUHNJ", 24, "Create an alert against the owner with water charges to tenant", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recG04HskXhugBh3t", 25, "If a Toop Manuel Real Estate property, add label", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recos0IotcRLoL6Z6", 28, "Save Management Agreement into PropertyMet", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recdcooKnVvx2IoR3", 30, "Review management agreement for missed information", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recEG821TCHSQCp86", 31, "Enter/update owner details onto REX", "" ),
( "rec58orUTPPCa5hkG", "New management - Front of House", "recDyAreifMlLINmP", 32, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "recfVHvTxJnybchST", 1, "Complete Handover with General Manager", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "reccbKTPG6cLLqhg2", 2, "Send new management - Welcome email", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "recViq1UMSLYHu714", 3, "Management takeover - contact the current agent to arrange handover date", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "recJpHwXPfGp6EGw8", 4, "Management takeover - advise PM to diarise date for collection of documents", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "recKp75SGplCy4HnP", 5, "Switch Management takeover - seek approval from the GM to make takeover cost neutral", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "recX82yzpndDFwydS", 6, "Switch Management takeover - obtain copy of owner's statement from previous agent", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "recae7wF7LoERrUQ1", 7, "Switch Management takeover - save statement to PM", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "recIDsr0WabsVMMLu", 8, "Switch Management takeover - set task for Finance Manager", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "rec5Q2r1kl5Q5kyKm", 9, "Create marketing copy in TMP & REX", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "recG4yIeb0Q2VK7TS", 10, "Send link to toop.com.au to landlord to review Ad", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "recaEjS9HvPeCM1B0", 11, "Diarise to collect keys and appliance manuals from the owner", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "recvszII1NWChZT1j", 12, "Scan Appliance Manuals into PropertyMe", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "recQwraiFAHMSh7ao", 13, "3 sets of keys/remotes/access card have been received & registered in PropertyMe", "" ),
( "rec5pCUteKFxm8Nkt", "New management - Leasing Co-ordinator", "rect1tl6e2gUNG3Ma", 14, "Download checklist and save to PropertyMe before deleting records", "" ),
( "reckrmSdfKx35zqYo", "New management - Property Manager", "recXHh6Louvhn4oDe", 2, "Enter property into routine inspection spreadsheet", "" ),
( "reckrmSdfKx35zqYo", "New management - Property Manager", "reclDtOJ9Ip4GHNaP", 3, "Enter routine inspection date against property", "" ),
( "reckrmSdfKx35zqYo", "New management - Property Manager", "recH6tWK8BrVGtKfM", 4, "ToopFix - enter maintenance instructions into owner profile ", "" ),
( "reckrmSdfKx35zqYo", "New management - Property Manager", "recQiWD2nkhXAXX0p", 5, "Has landlord requested a C&PM package", "" ),
( "reckrmSdfKx35zqYo", "New management - Property Manager", "recA38kETKhAMolSX", 6, "C&PM Maintenance package ordered", "" ),
( "reckrmSdfKx35zqYo", "New management - Property Manager", "recsiAXlMKrzGEOJh", 7, "Download checklist and save to PropertyMe", "" ),
( "recBOXKJRV9wtZd5k", "New tenant - Ingoing Inspection", "recC36QNkyCtkEunS", 1, "Create an ingoing inspection in PropertyMe", "" ),
( "recBOXKJRV9wtZd5k", "New tenant - Ingoing Inspection", "rec71FQ0FW8YrGZl2", 3, "Diarise inspection in outlook calendar", "" ),
( "recBOXKJRV9wtZd5k", "New tenant - Ingoing Inspection", "recExtQTnWV6zte85", 4, "Inspection completed & reviewed", "" ),
( "recBOXKJRV9wtZd5k", "New tenant - Ingoing Inspection", "rec1xmp7FFruUV0Gt", 5, "Add an alert on the tenant invoice with ingoing meter reading", "" ),
( "recBOXKJRV9wtZd5k", "New tenant - Ingoing Inspection", "recFRfCJpJFCl052R", 6, "Master key template attached to ingoing report", "" ),
( "recBOXKJRV9wtZd5k", "New tenant - Ingoing Inspection", "recQngiuU8s2kGxqD", 7, "Report sent to tenant", "" ),
( "recBOXKJRV9wtZd5k", "New tenant - Ingoing Inspection", "recndTErTmHdXwjP4", 8, "Report sent to landlord", "" ),
( "recBOXKJRV9wtZd5k", "New tenant - Ingoing Inspection", "recfjkeZWDkTQrSo5", 9, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recD14d1wreEF6zzK", 1, "Written instructions received from the owner and saved to Property Me", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "rec6XAoWBOs7bvVx8", 2, "Property reactivated in Property Me", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recOLL3B5jntU3x1W", 3, "Lost date and reason removed against the owner", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recayrERgIynrB7yw", 4, "Reinstated date entered and reason ", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recuINfe3rWiMmbmx", 6, "Keys received, tagged and placed in cabinet & tenant keys in key box", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recDJyXHUicClLrKm", 7, "Add property onto routine inspection spreadsheet", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recSeN5B4hesM2DuA", 8, "Change of postal address required - SA Water", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recrrG5ZTw3SDjteM", 9, "Change of postal address required - Strata", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recx2iJAudEoT0vRT", 10, "Change of postal address required - Council", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recLMDvcKCv45IWUC", 11, "Update the Property Manager's name if required", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recYKHbvsO9RVUCUj", 12, "Send welcome back email to landlord", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recn655CskH87fwJ8", 14, "Does a Compliance and Preventative Maintenance package need to be reinstated", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recORnP0m61oigJzJ", 15, "Leasing team member advised to relet", "" ),
( "recnPTSIEucW8pZV6", "Reinstated management", "recgS9zkOCGahP63n", 16, "Download checklist & save to PropertyMe before deleting records", "" ),
( "rec3puMpgXTmmRq2L", "Rent Arrears - 16 days", "rec7LrssvbqSStpSb", 1, "Rent is 14 days - let landlord know", "" ),
( "rec3puMpgXTmmRq2L", "Rent Arrears - 16 days", "recIdccmXtWwwO4fG", 2, "Rent is 16 days in arrears:  Create Form 2 for Rent Breach", "" ),
( "rec3puMpgXTmmRq2L", "Rent Arrears - 16 days", "recLYTkfTigrboSYh", 3, "Form 2 sent to tenants", "" ),
( "rec3puMpgXTmmRq2L", "Rent Arrears - 16 days", "recN5uOQs2xKwziBH", 4, "Landlord advised Form 2 has been issued", "" ),
( "rec3puMpgXTmmRq2L", "Rent Arrears - 16 days", "recPg0LVqtMfXSTfc", 5, "Arrears remedied by effective date on Form 2", "" ),
( "rec3puMpgXTmmRq2L", "Rent Arrears - 16 days", "recgA6Lzw7llqMPrd", 6, "Arrears remain outstanding on effective date on Form 2", "" ),
( "rec3puMpgXTmmRq2L", "Rent Arrears - 16 days", "recpBUjbcbJK1s0r1", 7, "Landlord advised SACAT application being lodged", "" ),
( "rec3puMpgXTmmRq2L", "Rent Arrears - 16 days", "recxUxo9Sy8w5WhlW", 8, "Tenant advised SACAT application being lodged", "" ),
( "rec3puMpgXTmmRq2L", "Rent Arrears - 16 days", "recBRhPHUySPp1e4x", 9, "Commence application to SACAT", "" ),
( "rec3puMpgXTmmRq2L", "Rent Arrears - 16 days", "rec7v6U9NDzJiQQHL", 10, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "recEXG5QhwUQYVttL", 1, "Application prepared & submitted - copy saved to PropertyMe", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "rec4BC4x0cTaptyH3", 2, "Application invoice received & sent to finance to pay", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "reck03R1K0WbLu8UQ", 3, "Notice of hearing received and diarised", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "recteNjHSHItOXZwJ", 4, "Landlord notified and invited to attend the hearing", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "rec3o46N3WcEmfxBO", 5, "Prepare checklist & documents for hearing", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "recbuUnMvZyrhauu7", 6, "Hearing attended and attendance charged to landlord", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "reclwh8PfzKELRlJ3", 7, "Is the order for amounts claimed different.  If so, amend internal invoices", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "rec7FZy27acDIK4dB", 8, "Call landlord and advise of outcome", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "rectuXieTwnpmqW8M", 9, "SACAT / Magistrates Court order received", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "rec2AIK54vABkP4nT", 10, "Emailed/post a copy of the order to landlord", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "recQSWIFmPxJtUIiR", 11, "Order for rent arrears - create alert note re payment plan", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "receCYT2QMvAD4ZQ4", 12, "Order for possession? - commence vacating process", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "rec9h7oEDAMOhBFqB", 13, "Copy of order emailed to Corporate Support", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "recEDYNgREyAq4VJg", 14, "Is the bond being released immediately? Create receipt note for Finance Manager", "" ),
( "recjjTN3bn9GkpPJn", "SACAT Hearings", "recPK0DOdwrGWnvz9", 15, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "recNSil0qCrusfBIt", 1, "Confirm agreement signed by all parties", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "rec77wvLMQKxuXrwM", 2, "Email new tenant pack to tenant with copy of lease", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "rec5wdZa96NRIuBOi", 3, "Save a copy of tenancy agreement in PropertyMe", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "rec8BQyCkEbUSPxys", 4, "All pre-lease maintenance has been completed", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "rec1rf5gwrGHkjalt", 5, "Confirm the property has been cleaned to standard", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "recG607XtTmGpapTP", 6, "Confirm the grounds have been presented to standard", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "recfr6sfkeUSxkPak", 7, "Routine inspection fee - calculate", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "reckYzN7D9n93ncVG", 8, "Enter administration fee", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "recNF6sRzwiNTACK4", 9, "Scan application fee and reference scripts into Property Me", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "recb9qpjaVjI17EeO", 10, "Remove the property from your Pan list", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "recU0kXpritfi1G2V", 11, "Charge marketing in PropertyMe", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "recvMYGf5gKblzJDJ", 12, "SA Water - Change of postal address required", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "recTGSrMiayRc2gs8", 13, "Council - change of postal address required", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "rectGcoFRJQRBxlo1", 14, "Strata - Change of postal address required", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "recpQE3mEhMsUPUIK", 16, "Diarise to prepare key collection the day before", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "recYcG1iv0hrEL5EH", 17, "Create work order for tenant gift", "" ),
( "recuONcwG7nSwXNYk", "Tenant confirmed - New Management", "reccCMplR0l6145O5", 18, "Download checklist & save to Property Me", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recdgszpuHmQuJ6YW", 1, "Confirm tenancy agreement signed by all parties", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "rec76G1a9TKgTXLCi", 2, "Email new tenant pack to tenant with copy of lease", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recau48Bo6HBYi3EB", 3, "Save copy of tenancy agreement in PropertyMe for owner & tenant", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recMdSx5nzeXJw6JG", 4, "Add water charges alert to tenant invoice", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recl3CG9CYmJnbQ4B", 5, "All pre-lease maintenance has been completed", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recZZRbtU1HtWrs8G", 6, "Confirm the property has been cleaned to a satisfactory standard", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recwWNZhsYWlUDLGg", 7, "Confirm the grounds have been presented to an acceptable standard", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recv1ytqbU80MvBpE", 8, "Inspection fee to be amended", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recIn4WNOxvond0TK", 9, "Maintenance expenditure limit updated in accordance with fee structure", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recSisdN6MMYSwS4m", 10, "Has the property just been renovated ? Enter admin fee  ", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "rec4PcqzBnIJEeqcu", 12, "Scan application and reference scripts into PropertyMe & mark 'Not visible'", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "rectVSp68jVSWCGrD", 13, "Charge marketing in PropertyMe", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "reccTDAFgtaMceJL1", 14, "Remove property from PAN list & place 'Leased' sticker on sign", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "rect3x4egWRR9oS7u", 15, "Lease Break?   If so change vacating date", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "rec7I6NSgk249EhxS", 17, "Lease break? Advise previous tenant property has been relet", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "rec0SAfUEBv2MttPl", 19, "Diarise to prepare key collection the day before the tenancy starts", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recsgbwVoafoUM2QW", 20, "Check work orders to update tenancy details", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recxZ6V0Ht3gFAlXJ", 21, "Create work order for tenant gift", "" ),
( "recTUadxm4JaCRJxy", "Tenant confirmed - Relet", "recGLTGyml9l6xxWY", 22, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recK695XnaMlEIN7D", "Tenant starts - New Management", "recd5rzjIY39DXRjJ", 1, "Photocopy all keys, remotes, access cards ready for collection", "" ),
( "recK695XnaMlEIN7D", "Tenant starts - New Management", "recCJEEvVME8ItpTW", 2, "Have appliance manuals been emailed to the tenant", "" ),
( "recK695XnaMlEIN7D", "Tenant starts - New Management", "recQuEr3sD1qBi8HX", 4, "Set task for lease start date in PropertyMe", "" ),
( "recK695XnaMlEIN7D", "Tenant starts - New Management", "rectQYYCdoNQ7y1Gy", 6, "Did the tenant sign a key receipt?", "" ),
( "recK695XnaMlEIN7D", "Tenant starts - New Management", "rec12j5AXT4Sla0gg", 7, "Is this a letting only?", "" ),
( "recK695XnaMlEIN7D", "Tenant starts - New Management", "recs5I396RGXocz7e", 8, "Water invoice to be issued (not for a letting only)", "" ),
( "recK695XnaMlEIN7D", "Tenant starts - New Management", "rec6sGnvjDJK75oHP", 9, "Download checklist & save to PropertyMe", "" ),
( "recHgZI6oKbaclVf7", "Tenant starts - Relet", "recUmv1NPmv0tbIth", 1, "Photocopy all keys, remotes, access cards ready for collection", "" ),
( "recHgZI6oKbaclVf7", "Tenant starts - Relet", "recd52EAkPnG1zk6k", 2, "Have appliance manuals been emailed to the tenant", "" ),
( "recHgZI6oKbaclVf7", "Tenant starts - Relet", "recgrJjYLETD4Od3Z", 4, "Set task for lease start date In PropertyMe", "" ),
( "recHgZI6oKbaclVf7", "Tenant starts - Relet", "recLIv8RPpDi6aQiR", 6, "Did the tenant sign a key receipt?  ", "" ),
( "recHgZI6oKbaclVf7", "Tenant starts - Relet", "rec2yrYoNfu6CQYqR", 7, "Is this a letting only?", "" ),
( "recHgZI6oKbaclVf7", "Tenant starts - Relet", "recASkdSuuUZgBKba", 8, "Remove label 'arrears bond claim' from property", "" ),
( "recHgZI6oKbaclVf7", "Tenant starts - Relet", "recODJtRAlJkOkvoX", 9, "Water invoice to be issued (not for a letting only)", "" ),
( "recHgZI6oKbaclVf7", "Tenant starts - Relet", "reciMQFKOdscPUE0Z", 10, "Download checklist &  save to PropertyMe before deleting the records", "" ),
( "recoxn64Do6Toykyg", "Under Renovation", "recAnmj3Yy6s2SM7Q", 1, "Attach a label 'under renovation' ", "" ),
( "recoxn64Do6Toykyg", "Under Renovation", "rec6GRmDsCxHSGgNN", 2, "Remove admin fee against owner", "" ),
( "recoxn64Do6Toykyg", "Under Renovation", "recgS0K8VNTXtHT80", 3, "Follow up dates with landlord", "" ),
( "recoxn64Do6Toykyg", "Under Renovation", "recf0sBDbgDi8dQnU", 4, "Property ready to relet", "" ),
( "recoxn64Do6Toykyg", "Under Renovation", "recljEzQ1AGUftqdr", 5, "Add administration fee against owner", "" ),
( "recoxn64Do6Toykyg", "Under Renovation", "recdAdPHcLKKRvj1Y", 6, "Do not relet", "" ),
( "recoxn64Do6Toykyg", "Under Renovation", "recJEjXJK5mpj0BuD", 7, "Commence advertising", "" ),
( "recoxn64Do6Toykyg", "Under Renovation", "recodMMiE3LX5VFB0", 8, "Remove label against property in PropertyMe", "" ),
( "recoxn64Do6Toykyg", "Under Renovation", "rec0z9mZHmK9x4TK2", 9, "Download checklist & save to PropertyMe before deleting the records", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "recEij1DGvzrNdU4i", 1, "Notice received from tenant/landlord/SACAT possession order", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "reccCPvLCSUIrZ2KC", 2, "Record the vacating / lease break date in PropertyMe", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "recp93A1QqxPCL3Mj", 4, "Periodic tenancy?   Remove the rent review date from PropertyMe", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "recOgefQ2PurEiDFc", 5, "Do you need to advance routine inspection date?", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "recFGd9gzfGAc6ld0", 6, "Final inspection date set in PropertyMe & outlook calendar", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "reccPzrQ6uzPkI4h2", 7, "Send confirmation email to tenant with Form 2a or Form 3 if required", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "reca9sOD0wmohpGsf", 8, "Discuss with team members if recommendations for upgrades required", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "recmOSWulCtwGCA3B", 9, "Contact Landlord to advise tenant vacating, discuss special conditions ", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "recginZVWUiTCPOcy", 10, "Routine inspection options - virtual/physical", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "recC1lWORn3kL7SkS", 11, "Do not relet", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "recS5Yg9a5rAubxxS", 12, "Is the landlord renovating? Add label in PropertyMe", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "recBL3BlGUcCssRer", 14, "Commence Advertising", "" ),
( "recZTFJlaZYGrRYwd", "Vacating Tenant", "recK6hOOGKYUvXvih", 15, "Download checklist &  save to PropertyMe before deleting the records", "" ),
            };

        private void _Do_6(Func<string, bool> doneToConfirmContinue = null)
        {
            if (string.IsNullOrEmpty(_Namespace))
            {
                MessageBox.Show("Please provide a namespace:extensionClassName:objectPrefix");
                return;
            }

            if (!_Namespace.Contains(":"))
            {
                MessageBox.Show("Please provide a namespace:extensionClassName:objectPrefix");
                return;
            }

            var parts = _Namespace.SplitEx(':');
            var template = GetTemplatePath("Airtable");
            var now = DateTime.Now;
            var projects = (Array)_App.ActiveSolutionProjects;

            // 准备插入代码
            ProjectItem file = null;
            Window win = null;
            TextSelection ts = null;
            StringBuilder sb = null;

            var batchIndex = -1;
            var saved = true;

            Action<ProjectItem> _newFile = f =>
            {
                batchIndex++;
                file = f.ProjectItems.AddFromTemplate(template, $"{parts[1]}.cs");
                win = file.Open(Constants.vsViewKindCode);
                sb = new StringBuilder();

                ts = win.Document.Selection as TextSelection;
                // ts.EndOfDocument();

                // 插入生成日期
                sb.AppendLine(@"/// <summary>");
                sb.AppendLine($"/// {now}");
                sb.AppendLine(@"/// </summary>");

                // 插入 namespace 行
                sb.AppendLine("namespace " + parts[0]);
                sb.AppendLine("{");

                saved = false;
            };

            Action _saveAndClose = () =>
            {
                ts.EndOfDocument();
                ts.Insert(sb.ToString());
                ts.Insert("}");
                ts.NewLine();
                ts.SelectAll();

                win.Activate();
                win.Document.Activate();

                _App.ExecuteCommand("Edit.FormatDocument");

                win.Close(vsSaveChanges.vsSaveChangesYes);
                saved = true;
            };

            Func<string, string> _makeVariable = input =>
            {
                var myTI = new CultureInfo("en-AU", false).TextInfo;
                var rgx = new Regex("[^a-zA-Z0-9]");
                var normalisedItem = rgx.Replace(input, " ");
                normalisedItem = myTI.ToTitleCase(normalisedItem).Replace(" ", "");
                return normalisedItem;
            };

            if (projects.Length > 0)
            {
                foreach (Project p in projects)
                {
                    ProjectItem folder = p.ProjectItems
                            .AddFolder("_Airtable", Constants.vsProjectItemKindPhysicalFolder);

                    _newFile(folder);

                    var currentTrigger = string.Empty;

                    foreach (var item in _Tasks)
                    {
                        if (currentTrigger != item.TriggerName)
                        {
                            if (!string.IsNullOrWhiteSpace(currentTrigger))
                            {
                                sb.AppendLine("}");
                                sb.AppendLine("");
                            }

                            currentTrigger = item.TriggerName;

                            sb.AppendLine($"[Airtable(Id = \"{item.TriggerId}\", Name = \"{item.TriggerName}\")]");
                            sb.AppendLine($"public enum {_makeVariable(item.TriggerName)}");
                            sb.AppendLine("{");
                        }

                        sb.Append($"[Airtable(TriggerId = \"{item.TriggerId}\", TriggerName = \"{item.TriggerName}\", Id = \"{item.Id}\", Name = \"{item.Name}\"");

                        if (!string.IsNullOrWhiteSpace(item.NextTriggerId))
                        {
                            sb.Append($", NextTrigger = typeof({_makeVariable(_Triggers[item.NextTriggerId])})");
                        }

                        sb.AppendLine(")]");
                        sb.AppendLine($"At{_makeVariable(item.Name)} = {item.Ordinal},");
                    }

                    sb.AppendLine("}");
                    if (!saved) _saveAndClose();

                }
            }
        }

    }
}