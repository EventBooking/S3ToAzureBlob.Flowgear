﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            Flowgear.Nodes.TestHarness.NodesTestHarness.AttachNode(typeof(S3ToAzureBlob.S3ToAzureBlob), null, null);
            Console.ReadKey();
        }
    }
}
