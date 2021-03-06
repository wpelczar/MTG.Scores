﻿using MTG.Scores.DataAccess;
using MTG.Scores.Models.View;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;

namespace MTG.Scores.Controllers
{
  public class HomeController : Controller
  {
    private MtgContext db = new MtgContext();

    public ActionResult Index()
    {
      var players = db.Players.ToList();

      var rank = new List<RankingRecord>();

      foreach (var player in players)
      {
        var games = player.HomeMatches.Count + player.AwayMatches.Count;

        var homeWins = player.HomeMatches.Count(x => x.Player1Score == 2 && x.Player1Score > x.Player2Score);
        var awayWins = player.AwayMatches.Count(x => x.Player2Score == 2 && x.Player2Score > x.Player1Score);
        var wonMatches = homeWins + awayWins;
        var lostMatches = games - wonMatches;

        var wonPoints = player.HomeMatches.Sum(x => x.Player1Score) + player.AwayMatches.Sum(x => x.Player2Score);
        var lostPoints = player.HomeMatches.Sum(x => x.Player2Score) + player.AwayMatches.Sum(x => x.Player1Score);

        rank.Add(
          new RankingRecord {
            Matches = games,
            WonMatches = wonMatches,
            LostMatches = lostMatches,
            Name = player.Name,
            LostPoints = lostPoints,
            WonPoints = wonPoints,
            PointsRatio = wonPoints - lostPoints
          });
      }

      rank = rank.OrderByDescending(x => x.WonMatches)
                 .ThenBy(x => x.Matches)
                 .ThenByDescending(x => x.PointsRatio)
                 .ThenByDescending(x => x.WonPoints)
                 .ToList();
      
      return View(rank);
    }
  }
}