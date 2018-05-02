using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class Approve
    {
    }

    [Serializable]
    public class NavigatActivity1
    {
        private string activityID;

        public string ActivityID
        {
            get { return activityID; }
            set { activityID = value; }
        }
        private string activityName;

        public string ActivityName
        {
            get { return activityName; }
            set { activityName = value; }
        }
        private int activityType;

        public int ActivityType
        {
            get { return activityType; }
            set { activityType = value; }
        }
        private bool canBeReturned;

        public bool CanBeReturned
        {
            get { return canBeReturned; }
            set { canBeReturned = value; }
        }
        private List<NavigatCandidate1> candidates;

        public List<NavigatCandidate1> Candidates
        {
            get { return candidates; }
            set { candidates = value; }
        }
        private bool compelPass;

        public bool CompelPass
        {
            get { return compelPass; }
            set { compelPass = value; }
        }
        private List<ClientOpinion1> opinions;

        public List<ClientOpinion1> Opinions
        {
            get { return opinions; }
            set { opinions = value; }
        }
        private int runningStatus;

        public int RunningStatus
        {
            get { return runningStatus; }
            set { runningStatus = value; }
        }
    }

    [Serializable]
    public class ClientOpinion1
    {
        private string activityID;

        public string ActivityID
        {
            get { return activityID; }
            set { activityID = value; }
        }
        private string activityName;

        public string ActivityName
        {
            get { return activityName; }
            set { activityName = value; }
        }
        private bool canEdit;

        public bool CanEdit
        {
            get { return canEdit; }
            set { canEdit = value; }
        }
        private DateTime createDate;

        public DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }

        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string operationType;

        public string OperationType
        {
            get { return operationType; }
            set { operationType = value; }
        }
        private string opinContent;

        public string OpinContent
        {
            get { return opinContent; }
            set { opinContent = value; }
        }
        private string taskID;

        public string TaskID
        {
            get { return taskID; }
            set { taskID = value; }
        }
        private string user;

        public string User
        {
            get { return user; }
            set { user = value; }
        }
        private string userID;

        public string UserID
        {
            get { return userID; }
            set { userID = value; }
        }
    }

    [Serializable]
    public class NavigatCandidate1
    {
        private bool completed;

        public bool Completed
        {
            get { return completed; }
            set { completed = value; }
        }
        private string deptName;

        public string DeptName
        {
            get { return deptName; }
            set { deptName = value; }
        }
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
    }
}