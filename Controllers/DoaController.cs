using _211426_FinalProjectDOA.Data;
using _211426_FinalProjectDOA.Models;
using _211426_FinalProjectDOA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace _211426_FinalProjectDOA.Controllers
{
    [Authorize]
    public class DoaController : Controller
    {
        //Hola aqui se expone la programacion mas relevante y pesada del proyecto, al ver estos archivos podra ver la complejidad y objetivo de estos, 
        //estos me permitiran llegaran a mi objetivo y poder proceder con la base de datos aplicada apropiadamente.
        private readonly GameContext _context;
        private readonly IWebHostEnvironment _environment;

        public DoaController(GameContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Characters (Index)
        public async Task<IActionResult> Index(string sChar, string order)
        {
            // Start by querying all characters
            var characters = from Lista in _context.Characters
                             select Lista;

            // Sorting characters by name (or another property)
            ViewData["Order"] = String.IsNullOrEmpty(order) || order == "asc" ? "desc" : "asc";
            switch (order)
            {
                case "desc":
                    characters = characters.OrderByDescending(c => c.Name);
                    break;
                case "asc":
                default:
                    characters = characters.OrderBy(c => c.Name);
                    break;
            }

            // Apply search filter for character name
            if (!String.IsNullOrEmpty(sChar))
            {
                characters = characters.Where(c => c.Name.Contains(sChar));
            }

            // Create the ViewModel to pass to the View
            SearchVM ImAFighter = new SearchVM
            {
                sChar = sChar, // Preserve search term
                theCharacters = await characters.ToListAsync() // Fetch the filtered characters
            };

            return View(ImAFighter);
        }
        // GET: Characters/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CharacterVM item)
        {
            if (ModelState.IsValid)
            {
                string fileName = LoadImage(item);
                Character fighter = new Character()
                {
                    Name = item.Name,
                    Picture = fileName
                };

                _context.Add(fighter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }
        private string LoadImage(CharacterVM model)
        {
            string uniqueFileName = null;
            if (model.Picture != null)
            {
                string folderLoad = Path.Combine(_environment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Picture.FileName;
                string filePath = Path.Combine(folderLoad, uniqueFileName);
                using (var dataChannel = new FileStream(filePath, FileMode.Create))
                {
                    model.Picture.CopyTo(dataChannel);
                }
            }

            return uniqueFileName;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fighter = await _context.Characters.FindAsync(id);
            if (fighter == null)
            {
                return NotFound();
            }
            return View(fighter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Picture,Name,Moves")] Character fighter)
        {
            if (id != fighter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fighter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CharacterExists(fighter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fighter);
        }
        private bool CharacterExists(int id)
        {
            return _context.Characters.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await _context.Characters
                .FirstOrDefaultAsync(m => m.Id == id);

            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var character = await _context.Characters.FindAsync(id);

            if (character == null)
            {
                return NotFound();
            }

            // Optionally delete the associated image file, if it exists
            if (!string.IsNullOrEmpty(character.Picture))
            {
                string imagePath = Path.Combine(_environment.WebRootPath, "images", character.Picture);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();

            // Redirect to the Character Index or AddMove page for the character selection
            return RedirectToAction(nameof(Index));  // Or use AddMove page if you prefer
        }

        public IActionResult AddMove(int characterId)
        {
            // Get character information to display on the page
            var character = _context.Characters.FirstOrDefault(c => c.Id == characterId);
            if (character == null)
            {
                return NotFound();
            }

            ViewBag.CharacterId = characterId;
            ViewBag.CharacterName = character.Name;
            ViewBag.Moves = _context.Moves.Where(m => m.CharacterId == characterId).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMove(Move item)
        {
            if (ModelState.IsValid)
            {
          
                Move moves = new Move()
                {
                    Name = item.Name,
                    Input =item.Input,
                    Startup = item.Startup,
                    Active = item.Active,
                    Recovery = item.Recovery,
                    AccumulatedStartup = item.AccumulatedStartup,
                    BlockAdvantage = item.BlockAdvantage,
                    CharacterId = item.CharacterId
                };

                _context.Add(moves);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddMove), new { characterId = item.CharacterId });
            }
            return View(item);
        }
        // GET: EditMove
        public async Task<IActionResult> EditMove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var move = await _context.Moves.FindAsync(id);
            if (move == null)
            {
                return NotFound();
            }

            return View(move);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMove(int id, [Bind("Id,Name,Input,Startup,Active,Recovery,AccumulatedStartup,BlockAdvantage,CharacterId")] Move move)
        {
            if (id != move.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure the correct CharacterId is being set on the move before updating
                    var existingMove = await _context.Moves.FindAsync(id);
                    if (existingMove == null)
                    {
                        return NotFound();
                    }

                    // Update the existing move with the new values
                    existingMove.Name = move.Name;
                    existingMove.Input = move.Input;
                    existingMove.Startup = move.Startup;
                    existingMove.Active = move.Active;
                    existingMove.Recovery = move.Recovery;
                    existingMove.AccumulatedStartup = move.AccumulatedStartup;
                    existingMove.BlockAdvantage = move.BlockAdvantage;
                    existingMove.CharacterId = move.CharacterId; // Ensure CharacterId is preserved

                    _context.Update(existingMove);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MoveExists(move.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirect to the AddMove page for the character after the edit
                return RedirectToAction(nameof(AddMove), new { characterId = move.CharacterId });
            }

            return View(move);
        }

        // Helper method to check if the move exists
        private bool MoveExists(int id)
        {
            return _context.Moves.Any(e => e.Id == id);
        }

        // GET: DeleteMove
        public async Task<IActionResult> DeleteMove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var move = await _context.Moves
                .FirstOrDefaultAsync(m => m.Id == id);

            if (move == null)
            {
                return NotFound();
            }

            return View(move);
        }

        // POST: DeleteMove
        [HttpPost, ActionName("DeleteMove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMoveConfirmed(int id)
        {
            var move = await _context.Moves.FindAsync(id);

            if (move == null)
            {
                return NotFound();
            }

            _context.Moves.Remove(move); // Remove the move from the database
            await _context.SaveChangesAsync();

            // Redirect to the AddMove page for the character that the move belongs to
            return RedirectToAction(nameof(AddMove), new { characterId = move.CharacterId });
        }

        public async Task<IActionResult> CalculateUnholdables(int characterId, int frameAdvantage)
        {
            // Store the original frame advantage
            int FrameAdvantage = frameAdvantage;
            int RemainingAdvantage = FrameAdvantage;

            // Populate ViewBag with the list of characters for the dropdown
            ViewBag.Characters = await _context.Characters.ToListAsync();

            // Fetch all moves for the specified character
            var moves = await _context.Moves
                .Where(m => m.CharacterId == characterId)
                .ToListAsync();

            if (moves == null || !moves.Any())
            {
                ViewBag.Message = "No moves found for the selected character.";
                return View(new List<List<MoveVM>>());
            }

            // List to store valid move combinations
            var validCombinations = new List<List<Move>>();

            // Iterate through all moves as potential starting points
            foreach (var move in moves)
            {
                // **Step 1**: Handle the first move check
                if (move.Startup + move.AccumulatedStartup == RemainingAdvantage - 1)
                {
                    validCombinations.Add(new List<Move> { move });
                    continue; // Move on to the next move in the list
                }

                // **Step 2**: Handle multi-move combinations
                var currentCombination = new List<Move> { move };
                RemainingAdvantage = FrameAdvantage - (move.Startup + move.AccumulatedStartup); // Deduct only startup + accumulatedstartup

                // Try adding additional moves in sequence
                foreach (var nextMove in moves)
                {
                    // Check if the next move exceeds RemainingAdvantage
                    if (nextMove.Startup + nextMove.AccumulatedStartup > RemainingAdvantage - 1)
                    {
                        // If it exceeds, reset and break out of this combination attempt
                        RemainingAdvantage = FrameAdvantage;
                        currentCombination.Clear();
                        break;
                    }

                    // Add the next move to the combination
                    currentCombination.Add(nextMove);
                    RemainingAdvantage -= nextMove.Startup + nextMove.Active + nextMove.Recovery + nextMove.AccumulatedStartup;

                    // If the next move matches the condition, save the combination
                    if (RemainingAdvantage == 1 && IsStrike(nextMove))
                    {
                        validCombinations.Add(new List<Move>(currentCombination));
                        break; // Stop adding moves once the condition is met
                    }
                }

                // Reset RemainingAdvantage if no valid combination found
                RemainingAdvantage = FrameAdvantage;
            }

            // Transform valid combinations into MoveVMs for the view
            var result = validCombinations.Select(combination => combination.Select((m, index) => new MoveVM
            {
                Name = m.Name,
                Input = m.Input,
                BlockAdvantage = index == combination.Count - 1 ? m.BlockAdvantage : 0 // Show BlockAdvantage only for the last move
            }).ToList()).ToList();

            return View(result);
        }

        // Method to determine if a move is a strike
        private bool IsStrike(Move move)
        {
            return move.Input.Contains("P") || move.Input.Contains("K");
        }

    }
}
