#!/usr/bin/env python3
# -*- coding: utf-8 -*-

#
# Implements a class which simply interfaces to Yafu
#
# We implement SIQS in this but this can be extended to
# other factorisation methods supported by Yafu very
# simply.
#
# @CTFKris - https://github.com/sourcekris/RsaCtfTool/
#

import os
import pathlib
import re
import logging
from attacks.abstract_attack import AbstractAttack
import subprocess
from lib.keys_wrapper import PrivateKey
from lib.utils import timeout, TimeoutError


class SiqsAttack(object):
    def __init__(self, n):
        """Configuration"""
        self.logger = logging.getLogger("global_logger")
        self.yafubin = os.path.join(pathlib.Path(__file__).parent, "yafu")
        self.threads = 2  # number of threads
        self.maxtime = 180  # max time to try the sieve

        self.n = n
        self.p = None
        self.q = None

    def testyafu(self):
        """Test if yafu can be run"""

        try:
            yafutest = subprocess.check_output(
                [self.yafubin, "siqs(1549388302999519)"],
                timeout=self.timeout,
                stderr=subprocess.DEVNULL,
            )
        except:
            yafutest = b""

        return b"48670331" in yafutest

    def checkyafu(self):
        # check if yafu exists and we can execute it
        return os.path.isfile(self.yafubin) and os.access(self.yafubin, os.X_OK)

    def benchmarksiqs(self):
        # NYI
        # return the time to factor a 256 bit RSA modulus
        return

    def doattack(self):
        """Perform attack"""
        yafurun = subprocess.check_output(
            [
                self.yafubin,
                "siqs(" + str(self.n) + ")",
                "-siqsT",
                str(self.maxtime),
                "-threads",
                str(self.threads),
            ],
            timeout=self.timeout,
            stderr=subprocess.DEVNULL,
        )

        primesfound = []

        if b"input too big for SIQS" in yafurun:
            self.logger.info("[-] Modulus too big for SIQS method.")
            return

        for line in yafurun.splitlines():
            if re.search(b"^P[0-9]+ = [0-9]+$", line):
                primesfound.append(int(line.split(b"=")[1]))

        if len(primesfound) == 2:
            self.p = primesfound[0]
            self.q = primesfound[1]

        if len(primesfound) > 2:
            self.logger.warning("[*] > 2 primes found. Is key multiprime?")

        if len(primesfound) < 2:
            self.logger.error("[*] SIQS did not factor modulus.")

        return


class Attack(AbstractAttack):
    def __init__(self, timeout=60):
        super().__init__(timeout)
        self.speed = AbstractAttack.speed_enum["medium"]

    def can_run(self):
        yafubin = os.path.join(pathlib.Path(__file__).parent, "yafu")

        if not os.path.isfile(yafubin) and os.access(yafubin, os.X_OK):
            return False

        try:
            yafutest = subprocess.check_output(
                [yafubin, "siqs(1549388302999519)"],
                timeout=self.timeout,
                stderr=subprocess.DEVNULL,
            )
        except:
            yafutest = b""

        if not b"48670331" in yafutest:
            return False

        return True

    def attack(self, publickey, cipher=[], progress=True):
        """Try to factorize using yafu"""
        with timeout(self.timeout):
            try:
                if publickey.n.bit_length() > 1024:
                    self.logger.error(
                        "[!] Warning: Modulus too large for SIQS attack module"
                    )
                    return (None, None)

                siqsobj = SiqsAttack(publickey.n)

                siqsobj.checkyafu()
                siqsobj.testyafu()

                if siqsobj.checkyafu() and siqsobj.testyafu():
                    siqsobj.doattack()

                if siqsobj.p and siqsobj.q:
                    publickey.q = siqsobj.q
                    publickey.p = siqsobj.p
                    priv_key = PrivateKey(
                        int(publickey.p),
                        int(publickey.q),
                        int(publickey.e),
                        int(publickey.n),
                    )
                    return (priv_key, None)
            except TimeoutError:
                return (None, None)
        return (None, None)

    def test(self):
        from lib.keys_wrapper import PublicKey

        key_data = """-----BEGIN PUBLIC KEY-----
MDwwDQYJKoZIhvcNAQEBBQADKwAwKAIhAM7gDElzPMzEU1htubZ8KvfHomChbmwN
ZrJ1fw38h5l1AgMBAAE=
-----END PUBLIC KEY-----"""
        result = self.attack(PublicKey(key_data), progress=False)
        return result != (None, None)
