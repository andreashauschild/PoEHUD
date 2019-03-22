﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PoeHUD.Poe.Components
{
    [StructLayout(LayoutKind.Explicit)]
    public struct TriggerableBlockageStruct
    {
        [FieldOffset(0x8)]
        public long OwnerPtr;
        [FieldOffset(0x30)]
        public byte IsClosed;

    }

    public class TriggerableBlockage : StructuredRemoteMemoryObject<TriggerableBlockageStruct>, Component
    {
        public Entity Owner => GetObject<Entity>(Structure.OwnerPtr);

        public bool IsClosed => Address != 0 && Structure.IsClosed == 1;
    }
}
