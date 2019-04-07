using System;
using System.IO;
using System.Windows.Media;
using Psychomotor_Test;


public class Attempt_data
{
    private string music_file_name;
    public string EmergencyFileName { get; }
    private const string path_to_music_folder = Sound_Test.path_to_music_folder;
    private const string path_to_emergency_folder = Sound_Test.path_to_emergency_folder;
    private string _MusicVolume;

    public MediaPlayer Music { get; private set; }
    public MediaPlayer Emergency { get; private set; }
    public TimeSpan EmergencyStart { get; }
    public TimeSpan AttemptTime { get; set; }

    public string MusicVolume
    {
        get { return _MusicVolume; }
        set
        {
            if (value != "low" && value != "medium" && value != "high")
                throw new ArgumentException("Only 'low', 'medium' and 'high' is acceptable");          
            _MusicVolume = value;
        }
    }

	public Attempt_data(string song_name, string volume)
	{
        music_file_name = song_name;
        EmergencyFileName = getEmergencyName();
        MusicVolume = volume;
        EmergencyStart = getEmergencyStart();
        Music = new MediaPlayer();
        Emergency = new MediaPlayer();
        Music.Volume = 0;
        Emergency.Volume = 0;
        Music.Open(new System.Uri(path_to_music_folder + volume + '_' + music_file_name));
        Emergency.Open(new System.Uri(path_to_emergency_folder + EmergencyFileName));
	}

    public bool IsReactionCorrect(string reaction)
    {
        return EmergencyFileName.Contains(reaction);
    }

    private string getEmergencyName()
    {
        Random rand = new Random();
        string[] Emergency = Directory.GetFiles(path_to_emergency_folder);
        return Emergency[rand.Next(Emergency.Length)].Substring(path_to_emergency_folder.Length);   // Gives name of random emergency sound from specific directory
    }

    private TimeSpan getEmergencyStart()
    {
        Random rand = new Random();
        TimeSpan random_time = new TimeSpan(0, 0, 0, rand.Next(5, 13), rand.Next(0, 1000));      // Random time from 5.000 to 12.999
        return random_time;
    }


}
