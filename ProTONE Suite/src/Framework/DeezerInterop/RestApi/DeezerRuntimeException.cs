using System;

namespace OPMedia.DeezerInterop.RestApi
{
    public class DeezerRuntimeException : Exception
    {
        private string HttpReasonPhrase;

        public DeezerRuntimeException(string httpReasonPhrase)
        {
            this.HttpReasonPhrase = httpReasonPhrase;
        }

    }
}
