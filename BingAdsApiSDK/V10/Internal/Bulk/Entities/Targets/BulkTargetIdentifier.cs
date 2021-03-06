﻿//=====================================================================================================================================================
// Bing Ads .NET SDK ver. 10.4
// 
// Copyright (c) Microsoft Corporation
// 
// All rights reserved. 
// 
// MS-PL License
// 
// This license governs use of the accompanying software. If you use the software, you accept this license. 
//  If you do not accept the license, do not use the software.
// 
// 1. Definitions
// 
// The terms reproduce, reproduction, derivative works, and distribution have the same meaning here as under U.S. copyright law. 
//  A contribution is the original software, or any additions or changes to the software. 
//  A contributor is any person that distributes its contribution under this license. 
//  Licensed patents  are a contributor's patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// 
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
//  each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, 
//  prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// 
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
//  each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, 
//  sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// 
// (A) No Trademark License - This license does not grant you rights to use any contributors' name, logo, or trademarks.
// 
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
//  your patent license from such contributor to the software ends automatically.
// 
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, 
//  and attribution notices that are present in the software.
// 
// (D) If you distribute any portion of the software in source code form, 
//  you may do so only under this license by including a complete copy of this license with your distribution. 
//  If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
// 
// (E) The software is licensed *as-is.* You bear the risk of using it. The contributors give no express warranties, guarantees or conditions.
//  You may have additional consumer rights under your local laws which this license cannot change. 
//  To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, 
//  fitness for a particular purpose and non-infringement.
//=====================================================================================================================================================

using System;
using Microsoft.BingAds.V10.Bulk.Entities;
using Microsoft.BingAds.V10.Internal.Bulk.Mappings;

// ReSharper disable once CheckNamespace
namespace Microsoft.BingAds.V10.Internal.Bulk.Entities
{
    /// <summary>
    /// Reserved for internal use.
    /// </summary>
    public abstract class BulkTargetIdentifier : BulkEntityIdentifier
    {   
        internal Status? Status { get; set; }
        
        internal long? TargetId { get; set; }
        
        internal long? EntityId { get; set; }
        
        internal string EntityName { get; set; }

        internal string ParentEntityName { get; set; }
        
        internal Type TargetBidType { get; set; }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        protected internal abstract string EntityColumnName { get; }

        private static readonly IBulkMapping<BulkTargetIdentifier>[] Mappings =
        {
            new SimpleBulkMapping<BulkTargetIdentifier>(StringTable.Status,
                c => c.Status.ToBulkString(),
                (v, c) => c.Status = v.ParseOptional<Status>()
            ),
            
            new SimpleBulkMapping<BulkTargetIdentifier>(StringTable.Id,
                c => c.TargetId.ToBulkString(),
                (v, c) => c.TargetId = v.ParseOptional<long>()
            ),

            new SimpleBulkMapping<BulkTargetIdentifier>(StringTable.ParentId,
                c => c.EntityId.ToBulkString(),
                (v, c) => c.EntityId = v.ParseOptional<long>()
            ),

            new DynamicColumnNameMapping<BulkTargetIdentifier>(c => c.EntityColumnName,
                c => c.EntityName,
                (v, c) => c.EntityName = v
            ),           
 
            new ConditionalBulkMapping<BulkTargetIdentifier>(StringTable.Campaign, c => c is BulkAdGroupTargetIdentifier,
                c => c.ParentEntityName,
                (v, c) => c.ParentEntityName = v                
           )
        };

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        protected BulkTargetIdentifier(Type targetBidType)
        {
            TargetBidType = targetBidType;
        }

        internal override void WriteToRowValues(RowValues values, bool excludeReadonlyData)
        {
            this.ConvertToValues(values, Mappings);
        }

        internal override void ReadFromRowValues(RowValues values)
        {
            values.ConvertToEntity(this, Mappings);
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        public override bool Equals(BulkEntityIdentifier other)
        {
            var otherIdentifier = other as BulkTargetIdentifier;

            if (otherIdentifier == null)
            {
                return false;
            }
            var isNameNotEmpty = !string.IsNullOrEmpty(EntityName) && !string.IsNullOrEmpty(ParentEntityName);

            return
                GetType() == other.GetType() &&
                (EntityId == otherIdentifier.EntityId ||
                 (isNameNotEmpty &&
                  EntityName == otherIdentifier.EntityName &&
                  ParentEntityName == otherIdentifier.ParentEntityName));
        }

        internal override bool IsDeleteRow
        {
            get { return Status == V10.Bulk.Entities.Status.Deleted; }
        }
    }
}