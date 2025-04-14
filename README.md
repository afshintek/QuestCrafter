# QuestCrafter
Gamify real life with quests (challenging friedns)— built with ASP.NET Core Web API

This is a little project I'm building for fun and practice (no fun actually, just practice).

I got the idea from an AI-chat bot and it sounded challenging and kindda cool, and a good way to learn more about ASP.NET Core Web API and Entity Framework Core and etc.

---

## Project Description

**What is it?**  
A bare-bones ASP.NET Core API for a social challenge app. Users can create quests, join others, complete them with picture proof, rate participants (1-100), and view leaderboards.

**What’s built so far?**  
- **Auth**: Register and log in to get a JWT token.
- **Quests**: Create quests with titles, descriptions, deadlines, and up to 5 pics.
- **Participation**: Join quests, mark them complete with pics.
- **Rating**: Score completed participants (1-100).
- **Views**: See all quests, filtered lists, leaderboards, or single quest details.

## How to Use It

It’s an API, so use tools like Postman:

1. Register: `POST /api/auth/register` (send `username`, `password`).
2. Login: `POST /api/auth/login` (get JWT token).
3. Create quest: `POST /api/quest/create` (title, description, deadline, pics).
4. Join: `POST /api/quests/{questId}/join`.
5. Complete: `POST /api/quests/{participantId}/complete` (upload pics).
6. Rate: `POST /api/quests/{questId}/rate` (participantId, score).
7. View: `GET /api/quest/GetQuests`, `GET /api/quest/getquest/{id}`, `GET /api/quests/{questId}/ratings`.

## How to Run It Locally

1. **Clone**:  
   ```bash
   git clone <repo-url>

    Requirements:
        .NET 8 SDK
        SQLite (or your DB)
    Setup:
        Create appsettings.json with:
        json

{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=questcrafter.db"
  },
  "Jwt": {
    "Issuer": "yourdomain.com",
    "Audience": "yourdomain.com",
    "Key": "your-secret-key-32-chars-long!!"
  }
}
Run migrations (if using EF Core CLI):
bash

    dotnet ef database update

Install:
bash
dotnet restore
Run:
bash

    dotnet run
    Test:
    Hit https://localhost:5001/api/auth/register in Postman.

Note

No frontend yet—just API. Use Postman or curl to interact.

## ⚙️ Tech Stack (so far)

- **ASP.NET Core Web API (.NET 8)**
- **Entity Framework Core** (code-first, with complex relationships)
- **SQL Server** (or SQLite for testing)

I haven’t picked a frontend yet — maybe React, Blazor, or something else.  
For now, it’s backend-only and all about clean API design.

---

## 💡 Why I'm building it

I wanted something that:
- Forces me to use real-world Entity Framework Core relationships (like Quests, Steps, Players)
- Gives me practice with clean API structure
- Includes stuff like user roles, invitations, tracking progress, and some fun logic
- Could one day be a full-stack app

---

## 🤝 Contributing

Pull requests are absolutely welcome! Feel free to open an issue first to chat about ideas.
