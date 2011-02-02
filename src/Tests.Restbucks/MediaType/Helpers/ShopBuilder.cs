using System;
using Restbucks.MediaType;

namespace Tests.Restbucks.MediaType.Helpers
{
    public class ShopBuilder
    {
        private Uri uri;

        public ShopBuilder()
        {
            uri = new Uri("http://localhost/");
        }

        public ShopBuilder WithBaseUri(Uri value)
        {
            uri = value;
            return this;
        }
        
        public Shop Build()
        {
            return new Shop(uri);
        }
    }
}