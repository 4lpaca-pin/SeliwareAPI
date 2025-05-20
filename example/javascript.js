// no test :>

const { exec } = require('child_process');
const fs = require('fs');
const util = require("node:util");
const execAsync = util.promisify(child_process.exec);

function RandomString(length) {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
        counter += 1;
    }
    return result;
};

async function GetProcesses(name) {
    try {
        const { stdout, stderr } = await execAsync('tasklist', { encoding: 'utf8' });
        const lines = stdout.split('\n');
        const robloxProcesses = lines.filter(line => line.includes(name));
        const pids = robloxProcesses.map(line => {
            const columns = line.trim().split(/\s+/);
            return columns[1];
        });
        return pids;
    }
    catch (error) {
        return [];
    }
};

async function EndAPIProcess() { // End api process (ZovwareAPI.exe)
    let pids = await GetProcesses("ZovwareAPI");

    pids.forEach(id => {
        process.kill(id);
    });
};

async function Inject(pid) { // Inject into roblox
    let pids = pid?[pid] : await GetProcesses("RobloxPlayerBeta");

    pids.forEach(pid => {
       fs.writeFileSync('./executable/InjectProcess.lua', pid.toString()); 
    });
};

async function Execute(code) { // execute script
    fs.writeFileSync('./executable/' + RandomString(10) + '.lua' , code);
};

function Init() { // start api process
    exec('./ZovwareAPI.exe');
};
