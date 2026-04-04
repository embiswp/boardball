using BoardBall.Core;

namespace BoardBall.Api.Models;

public record PlaceStoneRequest(Player Player, int Row, int Column);
