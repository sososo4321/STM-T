using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.UIElements.UxmlAttributeDescription;
#if ENABLE_WINMD_SUPPORT
using Windows.Storage;
using Windows.Foundation.Diagnostics;
#endif
public class BetterJson : MonoBehaviour
{
    public Game game;
    private Character character;

    void Start()
    {
        // Find the Character script in the scene to use it later for accuracy state updates
        character = FindObjectOfType<Character>();
    }


    [System.Serializable]
    public class Game
    {
        public List<Round> Rounds;
    }

    [System.Serializable]
    public class Round
    {
        public float accuracy;
        public float responsetime;
    }

    public void AddRound(float accuracy, float responsetime)
    {
        if (game == null)
        {
            CreateGame();
        }

        Round round = new Round();
        round.accuracy = accuracy;
        round.responsetime = responsetime;
        game.Rounds.Add(round);

        Debug.Log("ROUNDS: " + game.Rounds.Count);
    }

    public void CreateGame()
    {
        Game newGame = new Game();
        newGame.Rounds = new List<Round>();
        game = newGame;
    }

    public async void SaveThisGameData(string filePath)
    {
#if ENABLE_WINMD_SUPPORT
        LoggingChannel lc = new LoggingChannel("Mod11", null, new Guid("4bd2826e-54a1-4ba9-bf63-92b73ea1ac4a"));
        lc.LogMessage("I made a message!");
        StorageFolder storeFolder;
        StorageFile storeFile;

        StorageFile destinationFile;
        StorageFolder downloadsFolder;
        try
        {

            try
            {
                downloadsFolder = await DownloadsFolder.CreateFolderAsync("ResearchFiles");
                string folderToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(downloadsFolder);
                ApplicationData.Current.LocalSettings.Values["folderToken"] = folderToken;
                destinationFile = await downloadsFolder.CreateFileAsync("preExperimentResults.txt");

                string json = JsonUtility.ToJson(game);
                await FileIO.WriteTextAsync(destinationFile, json);
                lc.LogMessage("Created Folder and file");

            }
            catch (Exception ex)
            {
                if (ApplicationData.Current.LocalSettings.Values["folderToken"] != null)
                {
                    string token = ApplicationData.Current.LocalSettings.Values["folderToken"].ToString();
                    downloadsFolder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
                    destinationFile = await downloadsFolder.GetFileAsync("preExperimentResults.txt");


                    string text = await FileIO.ReadTextAsync(destinationFile);
                    text = text.Replace("Filetext","");
                    string json = JsonUtility.ToJson(game);
                    await FileIO.WriteTextAsync(destinationFile, text + json);
                    lc.LogMessage("GET and add to File");
                    lc.LogMessage(text);
                }
                else { lc.LogMessage("Access Token wasn't made"); }
            }

        }
        catch (FileNotFoundException ex)
        {
            lc.LogMessage("Error while creating file: " + ex.Message);
        }
#endif



    }
}
