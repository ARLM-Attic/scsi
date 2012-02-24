using System;
using System.IO;
using System.Threading;
using Ata;
using Helper.IO;
using Scsi.Block;
using Scsi.Multimedia;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Runtime.CompilerServices;
using System.Security;
using Helper;

namespace Scsi
{
	static class Program
	{
		public static void Main(string[] args)
		{
			//DiagnoseCdromDevice(args[0]);
			TestMultimedia();
			//TestBlock();
			//TestAtaDevice();

			/*
			unsafe
			{
				ConveyanceSelfTestRecommendedPollingTimeInMinutes: 0x02
				DataStructureChecksum: 0xc3
				ErrorLoggingCapability: 0x01
				ExtendedSelfTestRecommendedPollingTimeInMinutes1: 0x8e
				ExtendedSelfTestRecommendedPollingTimeInMinutes2: 0x0000
				IsValid: true
				OfflineDataCollectionCapability: 0x73
				OfflineDataCollectionStatus: 0x00
				OfflineDataCollectionTimeNeededInSeconds: 0x0000
				Reserved: 0x000000001c8ce199
				SelfTestExecutionStatusByte: 0xf9
				ShortSelfTestRecommendedPollingTimeInMinutes: 0x01
				SmartCapability: 0x0003
				VendorSpecific1: 0x000000001c8ce020
				VendorSpecific2: 0x00
				VendorSpecific3: 0x00
				VendorSpecific4: 0x000000001c8ce1a2
			}
			//*/
		}

		private static void TestMultimedia()
		{
			/* IMPORTANT:
			 * Before testing, make SURE to change the ISO file path to a valid one for your machine!
			 */
			using (var file = new Win32FileStream(@"\\.\CdRom0", FileAccess.ReadWrite, FileShare.ReadWrite, FileMode.Open, FileOptions.None))
			using (var spti = new Win32Spti(file.SafeFileHandle, true, true))
			using (var cd = new MultimediaDevice(spti, true))
			{
				cd.Inquiry();

				cd.SetWriteParameters(new ModeSelect10Command(), cd.GetWriteParameters(new ModeSense10Command()));

				//var mps = cd.ModeSense<SupportedModePages>(new ModeSense06Command(PageControl.CurrentValues));
				IMultimediaDevice iMMD = cd;
				if (file.DeviceType != Win32DeviceType.Cdrom)
				{ throw new InvalidOperationException("The selected drive is not a CD/DVD drive. Please change the drive letter in the constructor of the Win32FileStream class."); }

				Console.WriteLine("You are about to begin writing to the media in: {0}", file);
				Console.WriteLine("Please ensure that the disk is empty (it is recommended to write to a CD-RW media that has been erased using the Blank() method).");
				Console.WriteLine("Press ENTER when ready or Ctrl-C to quit...");
				Console.ReadLine();
				cd.Interface.LockVolume();
				try
				{
					Console.WriteLine("Synchronizing the cache (in case something is waiting to be done)...");
					cd.SynchronizeCache(new SynchronizeCache10Command());
					Console.WriteLine();

					var discInfo = cd.ReadDiscInformation();
					Console.WriteLine("The state of the inserted disc is: {0}", discInfo.DiscStatus);
					Console.WriteLine("The state of the last session on this disc is: {0}", discInfo.LastSessionState);
					Console.WriteLine();

					Console.WriteLine("Setting the device to the highest supported speed...");
					//Set the drive to the highest speed
					cd.SetCDSpeed(new SetCDSpeedCommand(ushort.MaxValue, ushort.MaxValue, RotationControl.ConstantLinearVelocity));
					Console.WriteLine();

					Console.WriteLine("Dismounting medium...");
					cd.Interface.DismountVolume();

					if (cd.CurrentProfile == MultimediaProfile.CDRW) { Blank(cd); } //Erase the CD-RW

					Console.WriteLine("Getting write parameters...");
					var writeParams = cd.GetWriteParameters(new ModeSense06Command(PageControl.CurrentValues));
					writeParams.MultiSession = MultisessionType.Multisession;
					writeParams.DataBlockType = DataBlockType.Mode1;
					writeParams.WriteType = WriteType.TrackAtOnce;
					writeParams.SessionFormat = SessionFormat.CdromOrCddaOrOtherDataDisc;
					writeParams.TrackMode = TrackMode.Other;
					Console.WriteLine("Setting write parameters...");
					cd.SetWriteParameters(new ModeSelect10Command(false, true), writeParams);

					Console.WriteLine();
					try
					{
						var caps = cd.GetCapabilities(new ModeSense06Command(PageControl.CurrentValues));
						Console.WriteLine("Current write speed: {0:N0} bytes/second", caps.CurrentWriteSpeed * 1000 /*This is NOT 1024*/);
					}
					catch (Exception ex)
					{ Console.WriteLine("Could not read current speed: {0}", ex.Message); }

					//  UPDATE the file path here!
					string filePath = @"C:\Images\Temp.iso";

					ushort trackNumber = (ushort)(iMMD.FirstTrackNumber + iMMD.TrackCount - 1);
					Console.WriteLine("Opening file {0}...", filePath);
					//Open an ISO image file (BE SURE TO EDIT THIS PATH TO POINT TO YOUR FILE!)
					using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					{
						Console.WriteLine("Burning new track...");
						var buffer = new byte[cd.BlockSize];
						fs.Position = 0;
						using (var track = iMMD.OpenTrack(trackNumber))
						{
							//track.Write(buffer, 0, buffer.Length);

							while (fs.Position < fs.Length)
							{
								fs.Read(buffer, 0, buffer.Length);
								track.Write(buffer, 0, buffer.Length);

								Console.CursorLeft = 0;
								Console.Write("Burned {0:N0} of {1:N0} bytes ({2:P2})...", fs.Position, fs.Length, (double)fs.Position / fs.Length);
							}
							Console.WriteLine();
							Console.WriteLine("Burn successful.");
							Console.WriteLine();
						}
					}

					Console.WriteLine("Closing session...");
					cd.CloseTrackOrSession(new CloseSessionTrackCommand(true, TrackSessionCloseFunction.CloseSessionOrStopBGFormat, trackNumber));
					SenseData sense;
					while ((sense = cd.RequestSense()).SenseKey == SenseKey.NotReady && sense.AdditionalSenseCode == AdditionalSenseCode.LogicalUnitNotReady && sense.AdditionalSenseCodeQualifier == (AdditionalSenseCodeQualifier)7)
					{
						Console.CursorLeft = 0;
						Console.Write("{0:P2} complete", sense.SenseKeySpecific.ProgressIndication.ProgressIndicationFraction);
						Thread.Sleep(50);
					}
					Console.WriteLine();
					Console.WriteLine("Session closed.");
					Console.WriteLine();

					Console.WriteLine("Media burned successfully.");
					Console.WriteLine();
				}
				catch (Exception ex)
				{
					Console.WriteLine();
					Console.WriteLine("ERROR: {0}", ex.Message);
				}
				finally { cd.Interface.UnlockVolume(); }
			}
			Console.Write("Press any key to continue...");
			Console.ReadKey(true);
		}

		private static void Blank(MultimediaDevice cd)
		{
			//* Blank
			Console.WriteLine("Blanking disc (minimal)...");
			cd.Blank(new BlankCommand(BlankingType.BlankMinimal, true, 0));
			SenseData sense;
			Thread.Sleep(50);
			while ((sense = cd.RequestSense()).SenseKey == SenseKey.NotReady
				&& sense.AdditionalSenseCode == AdditionalSenseCode.LogicalUnitNotReady
				&& sense.AdditionalSenseCodeQualifier == (AdditionalSenseCodeQualifier)7)
			{
				Console.Write("{0:P2} done", sense.SenseKeySpecific.ProgressIndication.ProgressIndicationFraction);
				Console.CursorLeft = 0;
				Thread.Sleep(50);
			}
		}

		private static void TestBlock()
		{
			//This is DANGEROUS! Send the wrong command and you'll overwrite your hard disk and lose all your data!
			using (var file = new Win32FileStream(@"\\.\PhysicalDrive0", FileAccess.ReadWrite, FileShare.ReadWrite, FileMode.Open, FileOptions.None))
			using (var spti = new Win32Spti(file.SafeFileHandle, true))
			using (var hd = new BlockDevice(spti, true))
			{
				IScsiDevice @interface = hd;
				var inq = hd.Inquiry(new InquiryCommand(VitalProductDataPageCode.DeviceInformation));
				//Console.Write("Press ENTER when ready...");
				//Console.ReadLine();
				//var buffer = new byte[@interface.BlockSize * 128];
				//for (long lba = 0; lba < @interface.Capacity / @interface.BlockSize; lba += (uint)((ulong)buffer.Length / @interface.BlockSize)) { @interface.Read((lba * @interface.BlockSize), buffer, 0, buffer.Length, true); }
			}
		}

		//I'm currently working on ATA devices as well... this is just for my tests.
		//DO NOT USE IT as you might very well corrupt the entire disk!
		private static void TestAtaDevice()
		{
			/*
			System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.KeyValuePair<long, long>>[] diskExtents;
			using (var stream = new Helper.IO.Win32FileStream(@"\\.\C:", FileAccess.Read, FileShare.ReadWrite, FileMode.Open, FileOptions.Asynchronous))
			{ diskExtents = stream.GetVolumeDiskExtents(); }
			if (diskExtents.Length != 1)
			{ throw new NotSupportedException("Multi-partition volumes not supported."); }
			else
			{
				Ata.AtaDevice ata;
				int sectorSize;
				{
					var disk = new Helper.IO.Win32FileStream(string.Format(@"\\.\PhysicalDrive{0}", diskExtents[0].Key), FileAccess.ReadWrite, FileShare.ReadWrite, FileMode.Open, FileOptions.Asynchronous);
					ata = new Ata.AtaDevice(new Ata.Win32AtaPassThroughInterface(disk.SafeFileHandle, false), false);
					GC.SuppressFinalize(disk); //We don't want to close the handle, since it's passed to the constructor above
					sectorSize = disk.BytesPerSector;
					disk = null; //Just a check
				}

				var ataStream = new Ata.AtaStream(ata, false);
				using (var partition = new Helper.IO.SubStream(ataStream, diskExtents[0].Value.Key, diskExtents[0].Value.Value, diskExtents[0].Value.Value, false))
				{
					var buf = new byte[sectorSize * 120];
					for (long i = 0; i < partition.Length; i += sectorSize)
					{
						partition.Read(buf, 0, buf.Length);
					}
				}
			}
			//*/

			using (var file = new Win32FileStream(@"\\.\PhysicalDrive0", FileAccess.ReadWrite, FileShare.ReadWrite, FileMode.Open, FileOptions.None))
			using (var apti = new Win32Apti(file.SafeFileHandle, true))
			using (var dev = new AtaDevice(apti, true))
			{
				var buf = new byte[dev.LogicalSectorSize * 249];
				Console.Write("Press ENTER to start...");
				Console.ReadLine();
				double max = 0;
				for (; ; )
				{
					//var blockCount = dev.NativeMaximumAddressExt + 1;
					long totalRead = 0;
					var startTick = Environment.TickCount;
					var sectorsToRead = (byte)(buf.Length / dev.LogicalSectorSize);
					long totalElapsed = 0;
					for (; ; )
					{
						dev.ReadDma(0, sectorsToRead, buf, 0);
						totalRead += buf.Length;
						totalElapsed = Environment.TickCount - startTick;
						if (totalElapsed > 500) { break; }
					}
					var speed = totalRead / TimeSpan.FromTicks(totalElapsed).TotalSeconds;
					max = Math.Max(speed, max);
					Console.WriteLine("{0:N0} B/s read speed (max = {1:N0})", speed, max);
				}
			}
		}

		private static void DiagnoseCdromDevice(string devPath)
		{
#if NDEBUG
			try
#endif
			{
				using (var fs = new Win32FileStream(devPath, Win32FileAccess.MaximumAllowed | Win32FileAccess.Synchronize, FileShare.ReadWrite, FileMode.Open, Win32FileOptions.NoBuffering))
				using (var ata = new AtaDevice(new Win32Apti(fs.SafeFileHandle, true), false))
				using (var scsi = new MultimediaDevice(new Win32Spti(fs.SafeFileHandle, true, true), false))
				{
					//ata.SmartEnableOperations();

					//Console.WriteLine("ATA - SMART Read Data");
					//Console.WriteLine(ata.SmartReadData());

					//Console.WriteLine();

					Console.WriteLine("SCSI - Device INQUIRY:");
					Console.WriteLine(scsi.Inquiry());
					ulong lba = 0;
					var bytes = new byte[scsi.BlockSize * 1];
					Console.WriteLine("SCSI - Reading LBA {0:N0}, {1:N0} bytes", lba, bytes.Length);
					scsi.Read(true, lba, (uint)(bytes.Length / scsi.BlockSize), bytes, 0);
					Console.WriteLine(BitConverter.ToString(bytes));
				}
			}
#if NDEBUG
			catch (ScsiException ex) { Console.WriteLine(ex.SenseData); }
			catch (Exception ex) { Console.WriteLine(ex); }
#endif
		}
	}
}
