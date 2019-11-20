package com.samsontech.bluetoothplugin;

import android.Manifest;
import android.app.Fragment;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothServerSocket;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.ArrayList;
import java.util.Set;
import java.util.UUID;


public class BluetoothManager extends Fragment {

    // Constants.
    public static final String TAG = "Bluetooth_Manager_Fragment";
    private static final String CALLBACK_METHOD_DEVICEFOUND = "NewDeviceFound";

    // Unity context.
    String gameObjectName;

    public static BluetoothManager instance;

    public static BluetoothManager getInstance() {
        return instance;
    }

    public BluetoothAdapter btAdapter;

    //region Setup
    public static void start(String gameObjectName) {
        // Instantiate and add to Unity Player Activity.
        instance = new BluetoothManager();
        instance.gameObjectName = gameObjectName; // Store `GameObject` reference
        UnityPlayer.currentActivity.getFragmentManager().beginTransaction().add(instance, BluetoothManager.TAG).commit();
    }

    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setRetainInstance(true); // Retain between configuration changes (like device rotation)
    }

    public BluetoothManager() {
        btAdapter = BluetoothAdapter.getDefaultAdapter();
        PairedDeviceList = new ArrayList<>();
        NewDeviceList = new ArrayList<BluetoothDevice>();
    }
//endregion

    //region Utils
    public void ShowToast(String messaage)
    {
        Toast.makeText(getActivity(), messaage, Toast.LENGTH_LONG).show();
    }

    void SendUnityMessage(String methodName, String parameter)
    {
        UnityPlayer.UnitySendMessage(gameObjectName, methodName, parameter);
    }
//endregion

    //region Permissions
    public void PermissionChecks() {
        if (Build.VERSION.SDK_INT >= 23) {
            //if (ActivityCompat.checkSelfPermission(getActivity(),
            //Manifest.permission.ACCESS_COARSE_LOCATION)
            //!= PackageManager.PERMISSION_GRANTED
            //&&
            //ActivityCompat.checkSelfPermission(getActivity(),
            //Manifest.permission.ACCESS_FINE_LOCATION)
            //!= PackageManager.PERMISSION_GRANTED) {

            requestPermissions(new String[]{Manifest.permission.ACCESS_COARSE_LOCATION, Manifest.permission.ACCESS_FINE_LOCATION}, 1);
            //}
        } else {
            ShowToast("Permission Already Granted");
        }
    }


    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        if(requestCode == 1)
        {
            if (grantResults.length > 0
                    && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                //ShowToast("Location Permission Granted");
            }
            else {
                //ShowToast("Location Permissin Denied");
            }
        }
    }
//endregion

    //region Bluetooth Checks
    public String BtCheck()
    {
        if(btAdapter == null)
        {
            return "0";
        }
        else
        {
            return "1";
        }
    }

    public String BtCheckIsOn()
    {
        if(btAdapter.isEnabled())
        {
            ShowToast("Bluetooth Is On");
            return "1";
        }
        else
        {
            ShowToast("Turning On Bluetooth");
            TurnBtOn();
            return "0";
        }
    }
    public void TurnBtOn()
    {
        btAdapter.enable();
    }
    //endregion

    //region Bluetooth Pairing

    ArrayList<BluetoothDevice> PairedDeviceList;
    ArrayList<BluetoothDevice> NewDeviceList;

    public void EnableDiscoverable()
    {
        Intent dIntent =  new Intent(BluetoothAdapter.ACTION_REQUEST_DISCOVERABLE);
        dIntent.putExtra(BluetoothAdapter.EXTRA_DISCOVERABLE_DURATION, 300);
        startActivity(dIntent);
    }

    public String GetPairedDevices()
    {
        int index = 0;
        Set<BluetoothDevice> btDevices = btAdapter.getBondedDevices();

        if(btDevices.size() > 0) {
            String str = "";

            for (BluetoothDevice device : btDevices) {
                PairedDeviceList.add((device));
                if(index == btDevices.size())
                {
                    str += device.getName();
                }
                else {
                    str += device.getName() + ",";
                }
            }
            return str;
        }
        else
        {
            return "no devices";
        }
    }

    public void DiscoverDevices()
    {
        NewDeviceList.clear();
        IntentFilter i = new IntentFilter(BluetoothDevice.ACTION_FOUND);
        getActivity().registerReceiver(receiver, i);
        btAdapter.startDiscovery();
    }

    public void CancelDiscovery()
    {
        getActivity().unregisterReceiver(receiver);
        btAdapter.cancelDiscovery();
    }

    BroadcastReceiver receiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            if (BluetoothDevice.ACTION_FOUND.equals(action)) {
                BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
                NewDeviceList.add(device);
                //ShowToast(device.getName());
                SendUnityMessage(CALLBACK_METHOD_DEVICEFOUND, device.getName());
            }
        }
    };
    //endregion

    //region Bluetooth Connecting Devices

    UUID myUUID = UUID.fromString("862cd111-7592-4453-a1b9-3a388e18a5f5");
    public BluetoothDevice CurDevice;
    public BluetoothSocket Socket;
    public BluetoothServerSocket serverSocket;

    public boolean isReceiving = false;

    public String GetIsReceviving()
    {
        if (isReceiving)
        {
            return "1";
        }
        else
        {
            return "0";
        }
    }

    public void ReceivePair()
    {
        ShowToast("Receiving Connection");
        isReceiving = true;
        Thread Receive = new Thread()
        {
            @Override
            public void run() {
                try {
                    serverSocket = btAdapter.listenUsingRfcommWithServiceRecord("BattleShots",myUUID);
                }catch (IOException e)
                {
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            ShowToast("Failed To Create Server Socket");
                        }
                    });
                    return;
                }

                Socket = null;

                while(Socket == null)
                {
                    try {
                        Socket = serverSocket.accept();
                    }catch (IOException e)
                    {
                    }

                    if(isReceiving == false)
                    {
                        break;
                    }

                    if(Socket != null)
                    {
                        getActivity().runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                CurDevice = Socket.getRemoteDevice();
                                ShowToast("Connected To: " + CurDevice.getName());
                                ReceivedPairCallBack(CurDevice.getName());
                                SetupInputOutputStreams();
                            }
                        });
                        break;
                    }
                }
            }
        };
        Receive.run();
    }

    public void ReceivedPairCallBack(String _device)
    {
        SendUnityMessage("ReceivedPairCallBack", _device);
    }

    public void ConnectToDevice(String _device)
    {
        boolean skip = false;
        ShowToast("Connecting To Device: " + _device);
        isReceiving = false;
        CancelDiscovery();
        for(BluetoothDevice device : PairedDeviceList)
        {
            if(device.getName().equals(_device))
            {
                CurDevice = device;
                skip = true;
                break;
            }
        }

        if(!skip) {
            for (BluetoothDevice device : NewDeviceList) {
                if (device.getName().equals(_device)) {
                    CurDevice = device;
                }
            }
        }
        Thread Connect = new Thread()
        {
            @Override
            public void run() {
                try
                {
                    Socket = CurDevice.createRfcommSocketToServiceRecord(myUUID);
                }catch (IOException e)
                {
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            ShowToast("Failed To Create Socket");
                            PairedWithDeviceCallBack("0");
                            ReceiveData();
                        }
                    });
                }
                try
                {
                    Socket.connect();
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            ShowToast("Connected To: " + CurDevice.getName());
                            PairedWithDeviceCallBack("1");
                            SetupInputOutputStreams();
                        }
                    });
                }catch (IOException e)
                {
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            PairedWithDeviceCallBack("0");
                            ShowToast("Failed To Connect");
                            ReceiveData();
                        }
                    });
                }
            }
        };
        Connect.run();
    }

    public void PairedWithDeviceCallBack(String status)
    {
        SendUnityMessage("ConnectedToDeviceCallBack", status);
    }
    //endregion

    //region Bluetooth Send/Receive

    public InputStream inputStream;
    public OutputStream outputStream;

    public void SetupInputOutputStreams() {
        try {
            inputStream = Socket.getInputStream();
            outputStream = Socket.getOutputStream();
            ReceiveData();
        }catch (IOException e)
        {
            ShowToast("Failed To Setup Streams");
        }
    }

    public void ReceivedDataCallBack(String data)
    {
        SendUnityMessage("ReceivedDataCallBack", data);
    }

    public String Msg;
    public void ReceiveData()
    {
        Thread _ReceiveData = new Thread()
        {
            @Override
            public void run() {
                while(Socket.isConnected())
                {
                    final byte[] buffer = new byte[1024];
                    int bytes = 0;

                    try {
                        bytes = inputStream.read(buffer);

                    }catch (IOException e)
                    {
                        getActivity().runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                ShowToast("Error Receiving Data");
                            }
                        });
                    }

                    if(bytes > 0)
                    {
                        try {
                           Msg = new String(buffer, "UTF-8");
                        }catch (IOException e)
                        {
                            getActivity().runOnUiThread(new Runnable() {
                                @Override
                                public void run() {
                                    ShowToast("Failed To Convert Buffer To String");
                                }
                            });
                        }
                        getActivity().runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                ShowToast("Received Data");
                                ReceivedDataCallBack(Msg);
                            }
                        });
                    }
                }
            }
        };
        _ReceiveData.run();
    }

    public void DataSentCallBack(String status)
    {
        SendUnityMessage("DataSentCallBack", status);
    }

    public String DataToSend;
    public void SendData(String data)
    {
        DataToSend = data;
        Thread _SendData = new Thread()
        {
            @Override
            public void run() {
                try
                {
                    outputStream.write(DataToSend.getBytes());
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            ShowToast("Message Sent");
                            DataSentCallBack("1");
                        }
                    });
                }catch (IOException e)
                {
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            ShowToast("Failed To Send Message");
                            DataSentCallBack("0");
                        }
                    });
                }
            }
        };
        _SendData.run();
    }

    //endregion

}
