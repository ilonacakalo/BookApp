namespace WebAPI.Services;

public enum ProfileUpdateResult { Success, NotFound, EmailTaken, UsernameTaken }

public enum ChangePasswordResult { Success, NotFound, WrongPassword }