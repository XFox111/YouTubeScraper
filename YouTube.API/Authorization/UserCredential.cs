using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace YouTube.Authorization
{
	public class UserCredential : Google.Apis.Auth.OAuth2.UserCredential
	{
		/// <summary>
		/// Event is fired when new refresh token is recieved and the old one is no loger valid
		/// </summary>
		public event EventHandler RefreshTokenUpdated;

		/// <summary>Constructs a new credential instance.</summary>
		/// <param name="flow">Authorization code flow.</param>
		/// <param name="userId">User identifier.</param>
		/// <param name="token">An initial token for the user.</param>
		public UserCredential(IAuthorizationCodeFlow flow, string userId, TokenResponse token) : base(flow, userId, token) { }

		/// <summary>Constructs a new credential instance.</summary>
		/// <param name="flow">Authorization code flow.</param>
		/// <param name="userId">User identifier.</param>
		/// <param name="token">An initial token for the user.</param>
		/// <param name="quotaProjectId">The ID of the project associated 
		/// to this credential for the purposes of quota calculation and billing. Can be null.</param>
		public UserCredential(IAuthorizationCodeFlow flow, string userId, TokenResponse token, string quotaProjectId) : base(flow, userId, token, quotaProjectId) { }

		/// <summary>
		/// Refreshes the token by calling to
		/// <see cref="Google.Apis.Auth.OAuth2.Flows.IAuthorizationCodeFlow.RefreshTokenAsync"/>.
		/// Then it updates the <see cref="TokenResponse"/> with the new token instance.
		/// </summary>
		/// <param name="taskCancellationToken">Cancellation token to cancel an operation.</param>
		/// <returns><c>true</c> if the token was refreshed.</returns>
		public new async Task<bool> RefreshTokenAsync(CancellationToken taskCancellationToken)
		{
			if (Token.RefreshToken == null)
			{
				Logger.Warning("Refresh token is null, can't refresh the token!");
				return false;
			}

			// It's possible that two concurrent calls will be made to refresh the token, in that case the last one 
			// will win.
			var newToken = await Flow.RefreshTokenAsync(UserId, Token.RefreshToken, taskCancellationToken)
				.ConfigureAwait(false);

			Logger.Info("Access token was refreshed successfully");

			if (newToken.RefreshToken == null)
				newToken.RefreshToken = Token.RefreshToken;

			Token = newToken;

			RefreshTokenUpdated?.Invoke(this, null);
			return true;
		}
	}
}