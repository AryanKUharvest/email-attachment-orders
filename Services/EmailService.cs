using Microsoft.Graph;
using Microsoft.Graph.Models;
using email_attachment_orders.Dto;

namespace email_attachment_orders.Services
{
    public class EmailService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly ExcelParseService _excelParseService;
        private readonly string _mailbox;

        public EmailService(
            GraphAuthService graphAuthService,
            ExcelParseService excelParseService)
        {
            _graphClient = graphAuthService.getClient();
            _excelParseService = excelParseService;

            _mailbox = Environment.GetEnvironmentVariable("MAILBOX_ADDRESS")
                ?? throw new Exception("Mailbox not found");
        }

        public async Task<List<EmailOrderDto>> GetParsedEmailOrdersAfterLastFetchedAsync(
            DateTime lastFetchedAtUtc)
        {
            var response = new List<EmailOrderDto>();

            var emails = await GetEmailsAfterLastFetchedAsync(lastFetchedAtUtc);

            foreach (var email in emails)
            {
                if (string.IsNullOrWhiteSpace(email.Id))
                    continue;

                var parsedRows = new List<ExcelOrderDto>();

                var excelAttachments = await GetExcelAttachmentsAsync(email.Id);

                foreach (var attachment in excelAttachments)
                {
                    if (attachment.ContentBytes == null)
                        continue;

                    try
                    {
                        var rows = _excelParseService.ParseExcel(attachment.ContentBytes);
                        parsedRows.AddRange(rows);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Failed to parse attachment {attachment.Name}: {ex.Message}");
                    }
                }

                if (parsedRows.Count == 0)
                    continue;

                response.Add(new EmailOrderDto
                {
                    MessageId = email.Id,
                    SenderEmail = email.From?.EmailAddress?.Address ?? string.Empty,
                    Subject = email.Subject ?? string.Empty,
                    Body = email.Body?.Content ?? string.Empty,
                    Timestamp = email.ReceivedDateTime?.DateTime ?? DateTime.MinValue,
                    IsRead = email.IsRead ?? false,
                    Attachments = parsedRows
                });
            }

            return response;
        }

        public async Task<List<EmailOrderDto>> GetParsedEmailOrdersForTestingAsync()
        {
            var response = new List<EmailOrderDto>();

            var emails = await GetEmailsAsync();

            foreach (var email in emails)
            {
                if (string.IsNullOrWhiteSpace(email.Id))
                    continue;

                var parsedRows = new List<ExcelOrderDto>();

                var excelAttachments = await GetExcelAttachmentsAsync(email.Id);

                foreach (var attachment in excelAttachments)
                {
                    if (attachment.ContentBytes == null)
                        continue;

                    try
                    {
                        var rows = _excelParseService.ParseExcel(attachment.ContentBytes);
                        parsedRows.AddRange(rows);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Failed to parse attachment {attachment.Name}: {ex.Message}");
                        continue;
                    }
                }

                if (parsedRows.Count == 0)
                    continue;

                response.Add(new EmailOrderDto
                {
                    MessageId = email.Id,
                    SenderEmail = email.From?.EmailAddress?.Address ?? string.Empty,
                    Subject = email.Subject ?? string.Empty,
                    Body = email.Body?.Content ?? string.Empty,
                    Timestamp = email.ReceivedDateTime?.DateTime ?? DateTime.MinValue,
                    IsRead = email.IsRead ?? false,
                    Attachments = parsedRows
                });
            }

            return response;
        }

        public async Task<List<Message>> GetEmailsAsync()
        {
            var result = await _graphClient.Users[_mailbox]
                .Messages
                .GetAsync(config =>
                {
                    config.QueryParameters.Filter =
                        "hasAttachments eq true";

                    config.QueryParameters.Select = new[]
                    {
                        "id",
                        "subject",
                        "from",
                        "body",
                        "receivedDateTime",
                        "hasAttachments",
                        "isRead"
                    };

                    //config.QueryParameters.Orderby = new[]
                    //{
                    //    "receivedDateTime desc"
                    //};

                    config.QueryParameters.Top = 20;
                });

            return result?.Value ?? new List<Message>();
        }

        public async Task<List<Message>> GetEmailsAfterLastFetchedAsync(DateTime lastFetchedAtUtc)
        {
            string lastFetchedIso = lastFetchedAtUtc
                .ToUniversalTime()
                .ToString("yyyy-MM-ddTHH:mm:ssZ");

            var result = await _graphClient.Users[_mailbox]
                .Messages
                .GetAsync(config =>
                {
                    config.QueryParameters.Filter =
                        $"receivedDateTime gt {lastFetchedIso} and hasAttachments eq true";

                    config.QueryParameters.Select = new[]
                    {
                        "id",
                        "subject",
                        "from",
                        "body",
                        "receivedDateTime",
                        "hasAttachments",
                        "isRead"
                    };

                    config.QueryParameters.Orderby = new[]
                    {
                        "receivedDateTime asc"
                    };

                    config.QueryParameters.Top = 50;
                });

            return result?.Value ?? new List<Message>();
        }

        public async Task<List<FileAttachment>> GetExcelAttachmentsAsync(string messageId)
        {
            var result = await _graphClient.Users[_mailbox]
                .Messages[messageId]
                .Attachments
                .GetAsync();

            var excelAttachments = new List<FileAttachment>();

            foreach (var attachment in result?.Value ?? new List<Attachment>())
            {
                if (attachment is FileAttachment file &&
                    file.ContentBytes != null &&
                    IsExcelFile(file.Name))
                {
                    excelAttachments.Add(file);
                }
            }

            return excelAttachments;
        }

        private bool IsExcelFile(string? fileName)
        {
            return !string.IsNullOrWhiteSpace(fileName)
                && (
                    fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)
                    || fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase)
                );
        }
    }
}