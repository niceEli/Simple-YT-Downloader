using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

class Program
{
	static async Task Main (string[] args)
	{
		if (args.Length == 0)
		{
			Console.WriteLine("Please provide the YouTube video URL as a command-line argument.");
			return;
		}

		string videoUrl = args[0];

		try
		{
			var youtube = new YoutubeClient();
			var video = await youtube.Videos.GetAsync(videoUrl);
			var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
			var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

			if (streamInfo != null)
			{
				Console.WriteLine($"Downloading video: {video.Title}");

				string fileName = $"{video.Title}.mov";
				string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

				var totalBlocks = (int)(streamInfo.Size.Bytes / 128000);
				var progress = new Progress<double>(value => ReportProgress(value, totalBlocks, video));

				await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath, progress);

				Console.WriteLine("\nDownload complete!");
			}
			else
			{
				Console.WriteLine("No suitable video stream found.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred: {ex.Message}");
		}
	}

	static void ReportProgress (double value, int totalBlocks, Video video)
	{
		var currentBlocks = (int)(value * totalBlocks);

		Console.Clear();
		Console.WriteLine($"Downloading video: {video.Title}");
		Console.WriteLine($"Progress: {currentBlocks} Blocks / {totalBlocks} Blocks");

		if (currentBlocks == totalBlocks)
			Console.WriteLine(); // Move to the next line after completion
	}
}
