using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VSTS.Contract
{
	public struct Build
	{
		//
		// Summary:
		//     Build number/name of the build
		[DataMember(EmitDefaultValue = false)]
		public string BuildNumber { get; set; }
		//
		// Summary:
		//     Build number revision
		[DataMember(EmitDefaultValue = false)]
		public int? BuildNumberRevision { get; set; }
		//
		// Summary:
		//     Indicates whether the build has been deleted.
		[DataMember(EmitDefaultValue = false)]
		public bool Deleted { get; set; }
		//
		// Summary:
		//     Time that the build was completed
		[DataMember(EmitDefaultValue = false)]
		public DateTime? FinishTime { get; set; }
		//
		// Summary:
		//     Id of the build
		[DataMember(EmitDefaultValue = false)]
		[System.ComponentModel.DataAnnotations.KeyAttribute]
		public int Id { get; set; }
		[DataMember(EmitDefaultValue = false)]
		public bool? KeepForever { get; set; }
		//
		// Summary:
		//     Parameters for the build
		[DataMember(EmitDefaultValue = false)]
		public string Parameters { get; set; }
		//
		// Summary:
		//     Quality of the xaml build (good, bad, etc.)
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string Quality { get; set; }
		//
		// Summary:
		//     The current position of the build in the queue
		[DataMember(EmitDefaultValue = false)]
		public int? QueuePosition { get; set; }
		//
		// Summary:
		//     Time that the build was queued
		[DataMember(EmitDefaultValue = false)]
		public DateTime? QueueTime { get; set; }
		//
		// Summary:
		//     The build result
		[DataMember(EmitDefaultValue = false)]
		public Contract.BuildResult? Result { get; set; }
		//
		// Summary:
		//     Source branch
		[DataMember(EmitDefaultValue = false)]
		public string SourceBranch { get; set; }
		//
		// Summary:
		//     Source version
		[DataMember(EmitDefaultValue = false)]
		public string SourceVersion { get; set; }
		//
		// Summary:
		//     Time that the build was started
		[DataMember(EmitDefaultValue = false)]
		public DateTime? StartTime { get; set; }
		//
		// Summary:
		//     Status of the build
		[DataMember(EmitDefaultValue = false)]
		public Contract.BuildStatus? Status { get; set; }
		//
		// Summary:
		//     Gets a collection of tags associated with the build.
		public List<string> Tags { get; }
		//
		// Summary:
		//     Uri of the build
		[DataMember(EmitDefaultValue = false)]
		public Uri Uri { get; set; }
		//
		// Summary:
		//     REST url of the build
		[DataMember(EmitDefaultValue = false)]
		public string Url { get; set; }
		//
		// Summary:
		//     The name on whose behalf the build was queued
		[DataMember(EmitDefaultValue = false)]
		public string RequestedFor { get; set; }

		public override bool Equals(object obj)
		{
			return obj is Build && this == (Build)obj;
		}
		public override int GetHashCode()
		{
			return (BuildNumber        == null ? 0 : BuildNumber.GetHashCode())
				^ (BuildNumberRevision == null ? 0 : BuildNumberRevision.GetHashCode())
				^ (Deleted.GetHashCode())
				^ (FinishTime          == null ? 0 : FinishTime.GetHashCode())
				^ (Id.GetHashCode())
				^ (KeepForever         == null ? 0 : KeepForever.GetHashCode())
				^ (Parameters          == null ? 0 : Parameters.GetHashCode())
				^ (Quality             == null ? 0 : Quality.GetHashCode())
				^ (QueuePosition       == null ? 0 : QueuePosition.GetHashCode())
				^ (QueueTime           == null ? 0 : QueueTime.GetHashCode())
				^ (Result              == null ? 0 : Result.GetHashCode())
				^ (SourceBranch        == null ? 0 : SourceBranch.GetHashCode())
				^ (SourceVersion       == null ? 0 : SourceVersion.GetHashCode())
				^ (StartTime           == null ? 0 : StartTime.GetHashCode())
				^ (Status              == null ? 0 : Status.GetHashCode())
				^ (Tags                == null ? 0 : Tags.GetHashCode())
				^ (Uri                 == null ? 0 : Uri.GetHashCode())
				^ (Url                 == null ? 0 : Url.GetHashCode())
				^ (RequestedFor        == null ? 0 : RequestedFor.GetHashCode());
		}
		public static bool operator ==(Build x, Build y)
		{
			return x.GetHashCode() == y.GetHashCode();
		}
		public static bool operator !=(Build x, Build y)
		{
			return !(x == y);
		}
	}
}
