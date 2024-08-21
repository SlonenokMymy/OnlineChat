namespace OnlineChat.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OnlineChat.DataModels;

    [ApiController]
    [Route("[controller]")]
    public class ChannelsController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public ChannelsController(ChatDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetChatHistory/{chatId}")]
        public async Task<IActionResult> GetChatHistory(int chatId)
        {
            var ChatHistory = await _context.ChatHistories.Where(x => x.Chat.Id == chatId).Select(x => new ChatHistoryRow() { 
            Content = x.Content,
            User = x.User.Username,
            ChatId = x.Chat.Id
            })
            .ToListAsync();

            if (ChatHistory == null)
            {
                return NotFound();
            }
            else return Ok(ChatHistory);
        }

        private class ChatHistoryRow
        { 
            public int ChatId { get; set; }
            public string ChatName { get; set; } = string.Empty;
            public string User { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
        }

        [HttpGet("GetAllChats/{ownerId}")]
        public async Task<IEnumerable<ChatInfo>> GetAllChats(int ownerId)
        {
            return await _context.ChatInfos.Where(x => x.OwnerId == ownerId).ToListAsync();
        }

        [HttpPost("SaveChat")]
        public async Task<ActionResult<ChatInfo>> PostChatInfo(ChatInfo chatInfo)
        {
            _context.ChatInfos.Add(chatInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostChatInfo), new { id = chatInfo.Id }, chatInfo);
        }

        [HttpPost("SaveMessage")]
        public async Task<ActionResult<ChatHistory>> PostChatHistory(ChatHistory chatHistory)
        {
            chatHistory.User = await _context.Users.FindAsync(chatHistory.User.Id);
            chatHistory.Chat = await _context.ChatInfos.FindAsync(chatHistory.Chat.Id);
            _context.ChatHistories.Add(chatHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostChatHistory), new { id = chatHistory.Id }, chatHistory);
        }

        [HttpDelete("DeleteMsg/{msgId}")]
        public async Task<IActionResult> DeleteMsg(int msgId)
        {
            var chatHistory = await _context.ChatHistories.FindAsync(msgId);
            if (chatHistory == null)
            {
                return NotFound();
            }

            _context.ChatHistories.Remove(chatHistory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Conflict(ex.ToString());
            }

            return NoContent();
        }

        [HttpDelete("DeleteChat/{chatId}")]
        public async Task<IActionResult> DeleteChat(int chatId)
        {
            try
            {
                var chatHistory = await _context.ChatHistories.Where(x => x.Chat.Id == chatId).ToListAsync();
                if (chatHistory.Any())
                {
                    _context.ChatHistories.RemoveRange(chatHistory);
                }
                await _context.SaveChangesAsync();

                var chat = await _context.ChatInfos.FindAsync(chatId);

                _context.ChatInfos.Remove(chat);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Conflict(ex.ToString());
            }
            

            return NoContent();
        }
    }
}
