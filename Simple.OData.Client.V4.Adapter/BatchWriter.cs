﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.OData.Core;

namespace Simple.OData.Client.V4.Adapter
{
    public class BatchWriter : BatchWriterBase
    {
        private ODataBatchWriter _batchWriter;
        private ODataRequestMessage _requestMessage;
        private ODataMessageWriter _messageWriter;

        public BatchWriter(ISession session)
            : base(session)
        {
        }

        public override async Task StartBatchAsync()
        {
            _requestMessage = new ODataRequestMessage() { Url = new Uri(_session.Settings.UrlBase) };
            _messageWriter = new ODataMessageWriter(_requestMessage);
            _batchWriter = await _messageWriter.CreateODataBatchWriterAsync();
            await _batchWriter.WriteStartBatchAsync();
        }

        public override async Task<HttpRequestMessage> EndBatchAsync()
        {
            if (_pendingChangeSet)
                await _batchWriter.WriteEndChangesetAsync();
            await _batchWriter.WriteEndBatchAsync();
            var stream = await _requestMessage.GetStreamAsync();
            return CreateMessageFromStream(stream, _requestMessage.Url, _requestMessage.GetHeader);
        }

        protected override Task StartChangesetAsync()
        {
            return _batchWriter.WriteStartChangesetAsync();
        }

        protected override Task EndChangesetAsync()
        {
            return _batchWriter.WriteEndChangesetAsync();
        }

        protected override async Task<object> CreateOperationRequestMessageAsync(string method, string collection, Uri uri, string contentId)
        {
            return await CreateBatchOperationRequestMessageAsync(method, collection, uri, contentId);
        }

        private async Task<ODataBatchOperationRequestMessage> CreateBatchOperationRequestMessageAsync(
            string method, string collection, Uri uri, string contentId)
        {
            var message = await _batchWriter.CreateOperationRequestMessageAsync(method, uri, contentId);

            if (method == RestVerbs.Post || method == RestVerbs.Put || method == RestVerbs.Patch)
                message.SetHeader(HttpLiteral.ContentId, contentId);

            if (_session.Metadata.EntityCollectionRequiresOptimisticConcurrencyCheck(collection) &&
                (method == RestVerbs.Put || method == RestVerbs.Patch || method == RestVerbs.Delete))
            {
                message.SetHeader(HttpLiteral.IfMatch, EntityTagHeaderValue.Any.Tag);
            }

            return message;
        }
    }
}