package com.johnjoemcbob.wearcompass;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.support.v4.content.LocalBroadcastManager;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

public class NetworkChangeReceiver extends BroadcastReceiver {
    public static final String NETWORK_CHANGE_ACTION = "NetworkChangeAction";

    @Override
    public void onReceive(final Context context, final Intent intent) {
        Intent networkChangeIntent = new Intent(NETWORK_CHANGE_ACTION);
        networkChangeIntent.putExtra("isOnline", isOnline(context));
        LocalBroadcastManager.getInstance(context).sendBroadcast(networkChangeIntent);
    }

    private boolean isOnline(Context context) {
        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo netInfo = cm.getActiveNetworkInfo();
        return (netInfo != null && netInfo.isConnected());
    }
}
