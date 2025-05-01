using System;

namespace E_comrece.Settings
{
    public class StripeSettings
    {
        public string SecretKey { get; set; } = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")
                                             ?? throw new InvalidOperationException("STRIPE_SECRET_KEY not found in environment variables");
        public string PublishableKey { get; set; } = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY")
                                                 ?? throw new InvalidOperationException("STRIPE_PUBLISHABLE_KEY not found in environment variables");
        public string WebhookSecret { get; set; } = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET")
                                                 ?? throw new InvalidOperationException("STRIPE_WEBHOOK_SECRET not found in environment variables");
    }
}
