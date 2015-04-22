﻿using System;
using System.Collections.Generic;

#pragma warning disable 1591

namespace Simple.OData.Client
{
    public interface IODataAdapter
    {
        AdapterVersion AdapterVersion { get; }
        ODataPayloadFormat DefaultPayloadFormat { get; }
        string ProtocolVersion { get; set; }
        object Model { get; set; }

        string GetODataVersionString();
        string ConvertValueToUriLiteral(object value);

        IMetadata GetMetadata();
        ICommandFormatter GetCommandFormatter();
        IResponseReader GetResponseReader();
        IRequestWriter GetRequestWriter(Lazy<IBatchWriter> deferredBatchWriter);
        IBatchWriter GetBatchWriter();
    }
}