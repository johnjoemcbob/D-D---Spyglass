package com.johnjoemcbob.wearspy;

import static android.content.Context.POWER_SERVICE;
import static android.content.Context.SENSOR_SERVICE;

import android.Manifest;
import android.content.pm.PackageManager;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.PowerManager;
import android.support.v7.app.AppCompatActivity;
import android.support.v4.content.ContextCompat;
import android.util.Log;
import android.view.WindowManager;
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


public class MainActivity extends AppCompatActivity implements SensorEventListener {

    private static final int MY_PERMISSIONS_REQUEST_COARSE_LOCATION = 123;

    private SensorManager sensorManager;
    private Sensor gyroSensor;
    private Sensor rotationSensor;
    private TextView textView;

    private ImageView imageView;
    private String Gyro;
    private PowerManager.WakeLock wakeLock;
    private String Status = "";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        textView = (TextView) findViewById(R.id.textView);
        textView.setText("");
        imageView = (ImageView) findViewById(R.id.imageView);

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED)
        {
            requestPermissions(new String[]{Manifest.permission.ACCESS_COARSE_LOCATION}, MY_PERMISSIONS_REQUEST_COARSE_LOCATION);
            Status += "Req|";
        }
        else
        {
            StartAfterPermission();
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults) {
        switch (requestCode) {
            case MY_PERMISSIONS_REQUEST_COARSE_LOCATION: {
                if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    Status += "Gra|";
                    StartAfterPermission();
                } else {
                    // permission denied, Disable the functionality that depends on this permission.
                }
                return;
            }

            // other 'case' lines to check for other permissions your app might request
        }
    }

    float[] mGravity;
    float[] mGeomagnetic;

    @Override
    public void onSensorChanged(SensorEvent event) {
        if (event.sensor.getType() == Sensor.TYPE_GYROSCOPE) {
            Gyro = event.values[0] + "|" +
                    event.values[1] + "|" +
                    event.values[2];
            //textView.setText(Status + "\n" + Gyro);
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {
        // You might want to implement behavior here if you care about sensor accuracy changes
        if (sensor.getType() == Sensor.TYPE_GYROSCOPE) {
            //Status = String.valueOf(accuracy);
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        sensorManager.registerListener(this, gyroSensor, SensorManager.SENSOR_DELAY_NORMAL);

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
        sensorManager.unregisterListener(this);

        // Always release the WakeLock when it's not needed
        if (wakeLock != null && wakeLock.isHeld()) {
            wakeLock.release();
            wakeLock = null;
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        handler.removeCallbacks(refreshRunnable);
        sensorManager.unregisterListener(this);

        // Always release the WakeLock when it's not needed
        if (wakeLock != null && wakeLock.isHeld()) {
            wakeLock.release();
            wakeLock = null;
        }
    }

    public void StartAfterPermission() {
        Status += "Sta|";
        sensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
        gyroSensor = sensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE);

        List<Sensor> deviceSensors = sensorManager.getSensorList(Sensor.TYPE_ALL);

        for (Sensor sensor : deviceSensors) {
            Log.i("SensorList", "Sensor name: " + sensor.getName() + ", type: " + sensor.getType());
        }

        if (gyroSensor == null) {
            textView.setText("This device has no gyroscope");
            finish();
        } else {
            sensorManager.registerListener(this, gyroSensor, SensorManager.SENSOR_DELAY_FASTEST);
        }
        Status += "Lis|";

        // Stay awake
        PowerManager powerManager = (PowerManager) getSystemService(POWER_SERVICE);
        wakeLock = powerManager.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "MyApp::MyWakelockTag");
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        // Acquire the WakeLock to keep the CPU running
        wakeLock.acquire();
        Status += "Loc|";

        Gyro = "0|0|0";
        new DownloadImageTask().execute();

        handler.post(refreshRunnable);
    }

    private void refreshGyro() {
        sensorManager.unregisterListener(this);
        sensorManager.registerListener(this, gyroSensor, SensorManager.SENSOR_DELAY_NORMAL);
        //Status += "Gyr|";
    }

    private Handler handler = new Handler();

    private Runnable refreshRunnable = new Runnable() {
        @Override
        public void run() {
            refreshGyro();
            handler.postDelayed(this, 1 * 1000);  // 1 seconds
        }
    };

    private class DownloadImageTask extends AsyncTask<Void, Bitmap, Void> {

        @Override
        protected Void doInBackground(Void... voids) {
            DatagramSocket socket = null;
            Bitmap bitmap = null;
            System.out.println("try");
            try {
                int serverPort = 7691;
                InetAddress host = InetAddress.getByName("192.168.0.26");
                Status += "Try|";
                System.out.println("Connecting to server " + host.getHostAddress() + " on port " + serverPort);

                socket = new DatagramSocket();
                socket.setSoTimeout(1000);
                socket.connect(new InetSocketAddress(host, serverPort));

                System.out.println("Just connected to " + socket.getRemoteSocketAddress());
                Status += "Con|";

                while (!isCancelled()) {
                    byte[] sendBuffer = Gyro.getBytes();
                    DatagramPacket sendPacket = new DatagramPacket(sendBuffer, sendBuffer.length, host, serverPort);
                    socket.send(sendPacket);
                    //Status += "Sen|";

                    // Buffer for reading data
                    byte[] buffer = new byte[10240];
                    DatagramPacket receivePacket = new DatagramPacket(buffer, buffer.length);

                    socket.receive(receivePacket);
                    //Status += "Rec|";
                    byte[] receivedData = receivePacket.getData();

                    // Extract length prefix
                    ByteBuffer wrapped = ByteBuffer.wrap(receivedData, 0, 4); // big-endian by default
                    int num = wrapped.getInt();

                    if (num > 0 && num <= (receivedData.length - 4)) {
                        bitmap = BitmapFactory.decodeByteArray(receivedData, 4, num);
                        //Status += "Bit|";
                        publishProgress(bitmap);
                    }
                }
            } catch (SocketTimeoutException e) {

            } catch (Exception e) {
                e.printStackTrace();
            } finally {
                if(socket != null) {
                    socket.close();
                }
                new DownloadImageTask().execute();
            }
            return null;
        }

        @Override
        protected void onProgressUpdate(Bitmap... bitmap) {
            if (bitmap[0] != null) {
                //Status += "Map|";
                imageView.setImageBitmap(bitmap[0]);
            }
        }
    }
}
