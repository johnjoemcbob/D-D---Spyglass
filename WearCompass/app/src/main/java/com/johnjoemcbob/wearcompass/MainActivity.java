package com.johnjoemcbob.wearcompass;

import static android.content.Context.POWER_SERVICE;
import static android.content.Context.SENSOR_SERVICE;

import android.Manifest;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;
import android.os.Handler;
import android.os.PowerManager;
import android.support.v7.app.AppCompatActivity;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v4.content.ContextCompat;
import android.util.Log;
import android.view.View;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.TextView;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.os.Bundle;
import android.widget.ImageView;

import java.io.*;
import java.net.*;
import java.nio.ByteBuffer;
import java.util.List;
import java.util.Timer;
import java.util.TimerTask;


public class MainActivity extends AppCompatActivity {

    private TextView textView;
    private ImageView imageView;
    private Button buttonRecenter;

    private PowerManager.WakeLock wakeLock;
    private Timer timer;

    private String Status = "";
    private boolean Clicked = false;
    private boolean Connected = false;
    private boolean Running = false;
    private Integer Angle = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        textView = (TextView) findViewById(R.id.textView);
        textView.setText("");
        imageView = (ImageView) findViewById(R.id.imageView);
        buttonRecenter = (Button) findViewById(R.id.buttonRecenter);
        buttonRecenter.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                Clicked = true;
            }
        });

        timer = new Timer();
        timer.scheduleAtFixedRate(new TimerTask() {
            @Override
            public void run() {
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        updateUI();

                        if ( !Running ){
                            //new NetworkTask().execute();
                        }
                    }
                });
            }
        }, 0, 100); // Update every 0.1 second

        LocalBroadcastManager.getInstance(this).registerReceiver(networkChangeReceiver, new IntentFilter(NetworkChangeReceiver.NETWORK_CHANGE_ACTION));

        StartAfterPermission();
    }

    public void updateUI(){
        textView.setText("");
        if ( !Connected ){
            textView.setText(Status);
        }
        imageView.setRotation(180 + Angle);
    }

    @Override
    protected void onResume() {
        super.onResume();

        // Stay awake
        PowerManager powerManager = (PowerManager) getSystemService(POWER_SERVICE);
        wakeLock = powerManager.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "MyApp::MyWakelockTag");
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        // Acquire the WakeLock to keep the CPU running
        wakeLock.acquire(4*60*60*1000L /*4 hours*/);
    }

    @Override
    protected void onPause() {
        super.onPause();

        // Always release the WakeLock when it's not needed
        if (wakeLock != null && wakeLock.isHeld()) {
            wakeLock.release();
            wakeLock = null;
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();

        LocalBroadcastManager.getInstance(this).unregisterReceiver(networkChangeReceiver);

        // Always release the WakeLock when it's not needed
        if (wakeLock != null && wakeLock.isHeld()) {
            wakeLock.release();
            wakeLock = null;
        }
    }

    public void StartAfterPermission() {
        //Status += "Sta|";

        //Status += "Lis|";

        // Stay awake
        PowerManager powerManager = (PowerManager) getSystemService(POWER_SERVICE);
        wakeLock = powerManager.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "MyApp::MyWakelockTag");
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        // Acquire the WakeLock to keep the CPU running
        wakeLock.acquire();
        //Status += "Loc|";

        new NetworkTask().execute();
    }

    private BroadcastReceiver networkChangeReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            boolean isOnline = intent.getBooleanExtra("isOnline", false);
            if (isOnline) {
                //imageView.setImageAlpha( 255 );
            } else {
                //imageView.setImageAlpha( 120 );
            }
        }
    };

    private class NetworkTask extends AsyncTask<Void, Integer, Void> {

        @Override
        protected Void doInBackground(Void... voids) {
            DatagramSocket socket = null;

            Running = true;

            //while (!isCancelled())
            {
                System.out.println("try");
                try {
                    int serverPort = 7691;
                    InetAddress host = InetAddress.getByName("192.168.0.26");
                    Status += "Try|";
                    System.out.println("Connecting to server " + host.getHostAddress() + " on port " + serverPort);

                    socket = new DatagramSocket();
                    socket.setSoTimeout(2000);
                    socket.connect(new InetSocketAddress(host, serverPort));

                    System.out.println("Just connected to " + socket.getRemoteSocketAddress());
                    Status += "Con|";

                    while (!isCancelled()) {
                        byte[] sendBuffer = "c".getBytes();
                        DatagramPacket sendPacket = new DatagramPacket(sendBuffer, sendBuffer.length, host, serverPort);
                        socket.send(sendPacket);
                        //Status += "Sen|";

                        if ( Clicked ){
                            sendBuffer = "r".getBytes();
                            sendPacket = new DatagramPacket(sendBuffer, sendBuffer.length, host, serverPort);
                            socket.send(sendPacket);

                            Clicked = false;
                        }

                        // Buffer for reading data
                        byte[] buffer = new byte[4];
                        DatagramPacket receivePacket = new DatagramPacket(buffer, buffer.length);

                        socket.receive(receivePacket);
                        //Status += "Rec|";
                        byte[] receivedData = receivePacket.getData();

                        // Extract length prefix
                        ByteBuffer wrapped = ByteBuffer.wrap(receivedData, 0, 4); // big-endian by default
                        Integer ang = wrapped.getInt();
                        Connected = true;
                        Angle = ang;
                    }
                } catch (SocketTimeoutException e) {

                } catch (Exception e) {
                    e.printStackTrace();
                } finally {
                    if(socket != null) {
                        socket.close();
                    }
                    new NetworkTask().execute();
                }
                Connected = false;
            }
            Running = false;
            return null;
        }

        @Override
        protected void onProgressUpdate(Integer... ang) {
            if (ang[0] != null) {
                //Status += "Map|";
                //imageView.setBackgroundColor( Color.TRANSPARENT );
            }
            else {
                //imageView.setBackgroundColor( Color.RED );
            }
        }
    }
}

