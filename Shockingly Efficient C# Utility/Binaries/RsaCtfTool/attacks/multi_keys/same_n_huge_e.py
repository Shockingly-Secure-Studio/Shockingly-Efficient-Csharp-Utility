#!/usr/bin/env python3
# -*- coding: utf-8 -*-

from attacks.abstract_attack import AbstractAttack
import tempfile
from Crypto.PublicKey import RSA
from lib.utils import timeout, TimeoutError


class Attack(AbstractAttack):
    def __init__(self, attack_rsa_obj, timeout=60):
        super().__init__(timeout)
        self.attack_rsa_obj = attack_rsa_obj
        self.speed = AbstractAttack.speed_enum["medium"]

    def attack(self, publickey, cipher=[], progress=True):
        """Same n huge e attack"""
        if not isinstance(publickey, list):
            return (None, None)

        with timeout(self.timeout):
            try:
                if len(set([_.n for _ in publickey])) == 1:
                    new_e = 1
                    for k in publickey:
                        new_e = new_e * k.e

                    new_key = (
                        RSA.construct((publickey[0].n, new_e)).publickey().exportKey()
                    )

                    tmpfile = tempfile.NamedTemporaryFile()
                    with open(tmpfile.name, "wb") as tmpfd:
                        tmpfd.write(new_key)
                        tmpfd.flush()
                        result = self.attack_rsa_obj.attack_single_key(tmpfile.name)
                        if result:
                            return (self.attack_rsa_obj.priv_key, None)
            except TimeoutError:
                return (None, None)
        return (None, None)
