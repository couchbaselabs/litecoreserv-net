//
// REST_native.cs
//
// Author:
// 	Jim Borden  <jim.borden@couchbase.com>
//
// Copyright (c) 2017 Couchbase, Inc All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Linq;
using System.Runtime.InteropServices;

using LiteCore.Util;

namespace LiteCore.Interop
{
#if LITECORE_PACKAGED
    internal
#else
    public
#endif 
    unsafe static partial class Native
    {
        [DllImport("LiteCoreREST", CallingConvention = CallingConvention.Cdecl)]
        public static extern C4RESTListener* c4rest_start(C4RESTConfig* config, C4Error* error);

        [DllImport("LiteCoreREST", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c4rest_free(C4RESTListener* listener);

        public static string c4rest_databaseNameFromPath(string path)
        {
			using (var path_ = new C4String(path))
			{
				using (var result = NativeRaw.c4rest_databaseNameFromPath(path_.AsC4Slice()))
				{
					return ((C4Slice)result).CreateString();
				}
			}
        }

        public static void c4rest_shareDB(C4RESTListener* listener, string name, C4Database* db)
        {
			using (var name_ = new C4String(name))
			{
				NativeRaw.c4rest_shareDB(listener, name_.AsC4Slice(), db);
			}
        }


    }
    
#if LITECORE_PACKAGED
    internal
#else
    public
#endif 
    unsafe static partial class NativeRaw
    {
        [DllImport("LiteCoreREST", CallingConvention = CallingConvention.Cdecl)]
        public static extern C4SliceResult c4rest_databaseNameFromPath(C4Slice path);

        [DllImport("LiteCoreREST", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c4rest_shareDB(C4RESTListener* listener, C4Slice name, C4Database* db);


    }
}
