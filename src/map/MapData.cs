namespace KitchenChaos;

using Chickensoft.Introspection;

[Meta, Id("map_data")]
public partial record MapData
{
  // [Save("coins_being_collected")]
  // public required Dictionary<string, CoinData> CoinsBeingCollected
  // {
  //   get; init;
  // }
  // [Save("collected_coin_ids")]
  // public required HashSet<string> CollectedCoinIds { get; init; }
}
