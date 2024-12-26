using DragonAPI.Enums;
using DragonAPI.Models.DAOs;
using System;
using System.Linq;

namespace DragonAPI.Extensions
{
    public static class DragonAccessibleFilterExtension
    {
        public static IQueryable<DragonDAO> FilterAccessibleDragonByWallets(this IQueryable<DragonDAO> queryable, string[] wallets)
        {
            if (wallets != null && wallets.Count() > 0)
            {
                return queryable.Where(r => r.SaleStatus == SaleStatus.NotSale && wallets.Contains(r.WalletId)
                                            || r.SaleStatus == SaleStatus.InWorkplace && !wallets.Contains(r.WalletId));
            }
            return queryable;
        }

        public static IQueryable<DragonDAO> FilterOwnDragonesByWallets(this IQueryable<DragonDAO> queryable, string[] wallets)
        {
            if (wallets != null && wallets.Count() > 0)
            {
                return queryable.Where(r => wallets.Contains(r.WalletId) || r.SaleStatus == SaleStatus.InWorkplace && !wallets.Contains(r.WalletId));
            }
            return queryable;
        }
    }
}