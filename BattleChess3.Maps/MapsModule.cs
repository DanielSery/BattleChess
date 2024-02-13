#region Copyright FEI Company 2024
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
#endregion

using Autofac;

namespace BattleChess3.Maps;

public static class MapsModule
{
    public static void RegisterMapsComponent(this ContainerBuilder builder)
    {
        builder.RegisterType<MapService>()
            .As<IMapService>()
            .SingleInstance();
        builder.RegisterType<FigureService>()
            .As<IFigureService>()
            .SingleInstance();
        builder.RegisterType<MapLoader>()
            .As<IMapLoader>()
            .SingleInstance();
    }
}